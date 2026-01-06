import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Card, CardPriority } from '../../models/card';
import { CardService } from '../../services/card';

export interface CardDetailsDialogData {
  card: Card;
}

@Component({
  selector: 'app-card-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './card-details.html',
  styleUrl: './card-details.scss'
})
export class CardDetailsComponent {
  cardForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  
  priorityOptions = [
    { value: CardPriority.None, label: 'No Priority', color: 'gray' },
    { value: CardPriority.Low, label: 'Low', color: 'green' },
    { value: CardPriority.Medium, label: 'Medium', color: 'yellow' },
    { value: CardPriority.High, label: 'High', color: 'orange' },
    { value: CardPriority.Critical, label: 'Critical', color: 'red' }
  ];

  constructor(
    private fb: FormBuilder,
    private cardService: CardService,
    public dialogRef: MatDialogRef<CardDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CardDetailsDialogData
  ) {
    this.cardForm = this.fb.group({
      title: [data.card.title, [Validators.required, Validators.maxLength(200)]],
      description: [data.card.description || '', [Validators.maxLength(2000)]],
      dueDate: [data.card.dueDate ? new Date(data.card.dueDate) : null],
      priority: [data.card.priority || CardPriority.None]
    });
  }

  onSave(): void {
    if (this.cardForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.errorMessage = '';

      const updatedCard: Partial<Card> = {
        title: this.cardForm.value.title,
        description: this.cardForm.value.description,
        dueDate: this.cardForm.value.dueDate,
        priority: this.cardForm.value.priority
      };

      this.cardService.updateCard(this.data.card.id, updatedCard).subscribe({
        next: (card) => {
          this.isLoading = false;
          this.dialogRef.close(card);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to update card. Please try again.';
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onDelete(): void {
    if (confirm('Are you sure you want to delete this card?')) {
      this.isLoading = true;
      this.cardService.deleteCard(this.data.card.id).subscribe({
        next: () => {
          this.isLoading = false;
          this.dialogRef.close({ deleted: true });
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to delete card. Please try again.';
        }
      });
    }
  }

  getPriorityColor(priority: CardPriority): string {
    const option = this.priorityOptions.find(p => p.value === priority);
    return option?.color || 'gray';
  }

  getPriorityLabel(priority: CardPriority): string {
    const option = this.priorityOptions.find(p => p.value === priority);
    return option?.label || 'No Priority';
  }

  getFieldError(fieldName: string): string {
    const field = this.cardForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['maxlength']) return `${fieldName} must be no more than ${field.errors['maxlength'].requiredLength} characters`;
    }
    return '';
  }
}