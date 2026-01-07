using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardMembersController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public BoardMembersController(TrelloDbContext context)
        {
            _context = context;
        }

        // GET: api/BoardMembers/board/{boardId}
        [HttpGet("board/{boardId}")]
        public async Task<ActionResult<IEnumerable<BoardMember>>> GetBoardMembers(int boardId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user has access to this board
            var hasAccess = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == currentUserId && bm.IsActive) ||
                await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == currentUserId);

            if (!hasAccess)
            {
                return Forbid();
            }

            var members = await _context.BoardMembers
                .Include(bm => bm.User)
                .Include(bm => bm.InvitedBy)
                .Where(bm => bm.BoardId == boardId && bm.IsActive)
                .OrderBy(bm => bm.Role)
                .ThenBy(bm => bm.JoinedAt)
                .ToListAsync();

            return Ok(members);
        }

        // POST: api/BoardMembers/{boardId}/invite
        [HttpPost("{boardId}/invite")]
        public async Task<ActionResult<BoardInvitation>> InviteMember(int boardId, InviteMemberRequest request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user can manage members for this board
            var canManage = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == currentUserId && 
                               bm.IsActive && bm.CanManageMembers) ||
                await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == currentUserId);

            if (!canManage)
            {
                return Forbid("You don't have permission to invite members to this board");
            }

            // Check if board exists
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null)
            {
                return NotFound("Board not found");
            }

            // Check if user is already a member
            var existingMember = await _context.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.User.Email == request.Email && bm.IsActive);

            if (existingMember != null)
            {
                return BadRequest("User is already a member of this board");
            }

            // Check if there's already a pending invitation
            var existingInvitation = await _context.BoardInvitations
                .FirstOrDefaultAsync(bi => bi.BoardId == boardId && bi.Email == request.Email && 
                                          bi.Status == InvitationStatus.Pending);

            if (existingInvitation != null)
            {
                return BadRequest("There's already a pending invitation for this email");
            }

            // Find invited user (if they exist)
            var invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // Create invitation
            var invitation = new BoardInvitation
            {
                BoardId = boardId,
                Email = request.Email,
                InvitedUserId = invitedUser?.Id,
                InvitedByUserId = currentUserId,
                ProposedRole = request.Role,
                Token = Guid.NewGuid().ToString("N"),
                Message = request.Message,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.BoardInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(invitation)
                .Reference(i => i.Board)
                .LoadAsync();
            await _context.Entry(invitation)
                .Reference(i => i.InvitedBy)
                .LoadAsync();

            return Ok(invitation);
        }

        // POST: api/BoardMembers/accept/{token}
        [HttpPost("accept/{token}")]
        public async Task<ActionResult<BoardMember>> AcceptInvitation(string token)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var invitation = await _context.BoardInvitations
                .Include(i => i.Board)
                .Include(i => i.InvitedBy)
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invitation == null)
            {
                return NotFound("Invitation not found");
            }

            if (!invitation.IsValid)
            {
                return BadRequest("Invitation is expired or invalid");
            }

            if (invitation.Email != userEmail)
            {
                return Forbid("This invitation is not for your email address");
            }

            // Check if user is already a member
            var existingMember = await _context.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == invitation.BoardId && bm.UserId == currentUserId && bm.IsActive);

            if (existingMember != null)
            {
                return BadRequest("You are already a member of this board");
            }

            // Create board membership
            var boardMember = new BoardMember
            {
                BoardId = invitation.BoardId,
                UserId = currentUserId,
                Role = invitation.ProposedRole,
                InvitedByUserId = invitation.InvitedByUserId,
                InvitedAt = invitation.CreatedAt,
                AcceptedAt = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.BoardMembers.Add(boardMember);

            // Update invitation status
            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;
            invitation.InvitedUserId = currentUserId;

            await _context.SaveChangesAsync();

            // Load user data for response
            await _context.Entry(boardMember)
                .Reference(bm => bm.User)
                .LoadAsync();

            return Ok(boardMember);
        }

        // DELETE: api/BoardMembers/{boardId}/{userId}
        [HttpDelete("{boardId}/{userId}")]
        public async Task<ActionResult> RemoveMember(int boardId, int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if current user can manage members
            var canManage = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == currentUserId && 
                               bm.IsActive && bm.CanManageMembers) ||
                await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == currentUserId);

            if (!canManage && currentUserId != userId)
            {
                return Forbid("You don't have permission to remove members from this board");
            }

            var member = await _context.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId && bm.IsActive);

            if (member == null)
            {
                return NotFound("Member not found");
            }

            // Check if trying to remove the board owner
            var isOwner = await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == userId);
            if (isOwner && currentUserId != userId)
            {
                return BadRequest("Cannot remove the board owner");
            }

            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Member removed successfully" });
        }

        // PUT: api/BoardMembers/{boardId}/{userId}/role
        [HttpPut("{boardId}/{userId}/role")]
        public async Task<ActionResult> UpdateMemberRole(int boardId, int userId, UpdateMemberRoleRequest request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if current user can manage members
            var canManage = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == currentUserId && 
                               bm.IsActive && bm.CanManageMembers) ||
                await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == currentUserId);

            if (!canManage)
            {
                return Forbid("You don't have permission to update member roles");
            }

            var member = await _context.BoardMembers
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId && bm.IsActive);

            if (member == null)
            {
                return NotFound("Member not found");
            }

            // Check if trying to change the board owner's role
            var isOwner = await _context.Boards.AnyAsync(b => b.Id == boardId && b.UserId == userId);
            if (isOwner)
            {
                return BadRequest("Cannot change the board owner's role");
            }

            member.Role = request.Role;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Member role updated successfully" });
        }
    }

    public class InviteMemberRequest
    {
        public string Email { get; set; } = string.Empty;
        public BoardRole Role { get; set; } = BoardRole.Member;
        public string? Message { get; set; }
    }

    public class UpdateMemberRoleRequest
    {
        public BoardRole Role { get; set; }
    }
}