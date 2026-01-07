import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from './auth';

export interface OnlineUser {
  userId: string;
  username: string;
  connectionId: string;
}

export interface CardMoveEvent {
  cardId: number;
  sourceListId: number;
  targetListId: number;
  newPosition: number;
}

export interface UserPresenceEvent {
  userId: string;
  username: string;
  connectionId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private currentBoardId: string | null = null;
  
  // Observable streams for real-time events
  private onlineUsersSubject = new BehaviorSubject<OnlineUser[]>([]);
  public onlineUsers$ = this.onlineUsersSubject.asObservable();
  
  private connectionStateSubject = new BehaviorSubject<HubConnectionState>(HubConnectionState.Disconnected);
  public connectionState$ = this.connectionStateSubject.asObservable();

  // Event streams
  private cardMovedSubject = new BehaviorSubject<CardMoveEvent | null>(null);
  public cardMoved$ = this.cardMovedSubject.asObservable();
  
  private cardUpdatedSubject = new BehaviorSubject<any>(null);
  public cardUpdated$ = this.cardUpdatedSubject.asObservable();
  
  private cardCreatedSubject = new BehaviorSubject<any>(null);
  public cardCreated$ = this.cardCreatedSubject.asObservable();
  
  private cardDeletedSubject = new BehaviorSubject<any>(null);
  public cardDeleted$ = this.cardDeletedSubject.asObservable();
  
  private listCreatedSubject = new BehaviorSubject<any>(null);
  public listCreated$ = this.listCreatedSubject.asObservable();
  
  private listDeletedSubject = new BehaviorSubject<any>(null);
  public listDeleted$ = this.listDeletedSubject.asObservable();
  
  private userTypingSubject = new BehaviorSubject<any>(null);
  public userTyping$ = this.userTypingSubject.asObservable();

  constructor(private authService: AuthService) {
    this.authService.authState$.subscribe(authState => {
      if (authState.isLoggedIn && authState.token) {
        this.startConnection(authState.token);
      } else {
        this.stopConnection();
      }
    });
  }

  private async startConnection(token: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5056/boardHub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.setupEventListeners();

    try {
      await this.hubConnection.start();
      this.connectionStateSubject.next(this.hubConnection.state);
      console.log('SignalR connection started');
    } catch (error) {
      console.error('Error starting SignalR connection:', error);
    }

    // Handle connection state changes
    this.hubConnection.onreconnecting(() => {
      this.connectionStateSubject.next(HubConnectionState.Reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionStateSubject.next(HubConnectionState.Connected);
      // Rejoin current board if we were in one
      if (this.currentBoardId) {
        this.joinBoard(this.currentBoardId);
      }
    });

    this.hubConnection.onclose(() => {
      this.connectionStateSubject.next(HubConnectionState.Disconnected);
    });
  }

  private setupEventListeners(): void {
    if (!this.hubConnection) return;

    // User presence events
    this.hubConnection.on('UserJoined', (data: UserPresenceEvent) => {
      const currentUsers = this.onlineUsersSubject.value;
      const updatedUsers = [...currentUsers, data];
      this.onlineUsersSubject.next(updatedUsers);
    });

    this.hubConnection.on('UserLeft', (data: UserPresenceEvent) => {
      const currentUsers = this.onlineUsersSubject.value;
      const updatedUsers = currentUsers.filter(user => user.connectionId !== data.connectionId);
      this.onlineUsersSubject.next(updatedUsers);
    });

    // Board update events
    this.hubConnection.on('CardMoved', (data: CardMoveEvent) => {
      this.cardMovedSubject.next(data);
    });

    this.hubConnection.on('CardUpdated', (data: any) => {
      this.cardUpdatedSubject.next(data);
    });

    this.hubConnection.on('CardCreated', (data: any) => {
      this.cardCreatedSubject.next(data);
    });

    this.hubConnection.on('CardDeleted', (data: any) => {
      this.cardDeletedSubject.next(data);
    });

    this.hubConnection.on('ListCreated', (data: any) => {
      this.listCreatedSubject.next(data);
    });

    this.hubConnection.on('ListDeleted', (data: any) => {
      this.listDeletedSubject.next(data);
    });

    this.hubConnection.on('UserTyping', (data: any) => {
      this.userTypingSubject.next(data);
    });
  }

  public async joinBoard(boardId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      this.currentBoardId = boardId;
      await this.hubConnection.invoke('JoinBoardGroup', boardId);
      // Reset online users when joining a new board
      this.onlineUsersSubject.next([]);
    }
  }

  public async leaveBoard(boardId: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('LeaveBoardGroup', boardId);
      this.currentBoardId = null;
      this.onlineUsersSubject.next([]);
    }
  }

  // Board update notifications
  public async notifyCardMoved(boardId: string, cardMoveData: CardMoveEvent): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyCardMoved', boardId, cardMoveData);
    }
  }

  public async notifyCardUpdated(boardId: string, cardData: any): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyCardUpdated', boardId, cardData);
    }
  }

  public async notifyCardCreated(boardId: string, cardData: any): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyCardCreated', boardId, cardData);
    }
  }

  public async notifyCardDeleted(boardId: string, cardData: any): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyCardDeleted', boardId, cardData);
    }
  }

  public async notifyListCreated(boardId: string, listData: any): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyListCreated', boardId, listData);
    }
  }

  public async notifyListDeleted(boardId: string, listData: any): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('NotifyListDeleted', boardId, listData);
    }
  }

  public async sendTypingIndicator(boardId: string, cardId: string, isTyping: boolean): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === HubConnectionState.Connected) {
      await this.hubConnection.invoke('SendTypingIndicator', boardId, cardId, isTyping);
    }
  }

  private async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = null;
      this.currentBoardId = null;
      this.connectionStateSubject.next(HubConnectionState.Disconnected);
      this.onlineUsersSubject.next([]);
    }
  }

  public getCurrentBoardId(): string | null {
    return this.currentBoardId;
  }

  public isConnected(): boolean {
    return this.hubConnection?.state === HubConnectionState.Connected;
  }
}