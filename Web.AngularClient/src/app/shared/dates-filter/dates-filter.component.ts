import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dates-filter',
  standalone: true,
  templateUrl: './dates-filter.component.html',
  styleUrls: ['./dates-filter.component.scss'],
  imports: [NgFor, FormsModule],
})
export class DatesFilterComponent implements OnInit {
  @Input() model: string | null = null;
  @Output() modelChange = new EventEmitter<string | null>();
  @Input() label: string = 'Фильтр по году/месяцу:';
  @Input() allLabel: string = 'Все';
  @Output() change = new EventEmitter<void>();

  public options: string[] = [];

  ngOnInit(): void {
    this.generateYearMonthOptions();
    if (!this.model || this.model === 'all') {
      this.model = '';
      this.modelChange.emit(this.model);
    }
  }

  generateYearMonthOptions(): void {
    this.options = [];
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1;
    for (let year = 2024; year <= currentYear; year++) {
      const endMonth = year === currentYear ? currentMonth : 12;
      for (let month = 1; month <= endMonth; month++) {
        const monthStr = month.toString().padStart(2, '0');
        this.options.push(`${year}.${monthStr}`);
      }
    }
    this.options.unshift(this.allLabel);
  }

  onModelChange(value: string) {
    this.model = value;
    this.modelChange.emit(value);
    this.change.emit();
  }
} 