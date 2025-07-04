import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';

export interface IVerificationsFilter {
  yearMonthFilter: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class VerificationsService {
  private currentFilters: IVerificationsFilter = {
    yearMonthFilter: 'all'
  };

  private pagination: Pagination;
  private yearMonthOptions: string[] = [];

  constructor() {
    this.pagination = new Pagination();
    this.generateYearMonthOptions();
  }

  public setPageChangeCallback(callback: () => void): void {
    this.pagination.setPageChangeCallback(callback);
  }

  public getYearMonthOptions(): string[] {
    return [...this.yearMonthOptions];
  }

  public getYearMonthFilter(): string | null {
    return this.currentFilters.yearMonthFilter;
  }

  public setYearMonthFilter(filter: string | null): void {
    this.currentFilters.yearMonthFilter = filter;
  }

  public getPagination(): Pagination {
    return this.pagination;
  }

  public updatePaginationFromData(data: IPaginationData): void {
    this.pagination.updateFromData(data);
  }

  public resetPagination(): void {
    this.pagination.reset();
  }

  public resetToFirstPage(): void {
    this.pagination.resetToFirstPage();
  }

  private generateYearMonthOptions(): void {
    this.yearMonthOptions = [];
    
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1;
    
    for (let year = 2024; year <= currentYear; year++) {
      const startMonth = year === 2024 ? 1 : 1;
      const endMonth = year === currentYear ? currentMonth : 12;
      
      for (let month = startMonth; month <= endMonth; month++) {
        const monthStr = month.toString().padStart(2, '0');
        this.yearMonthOptions.push(`${year}.${monthStr}`);
      }
    }
    
    this.yearMonthOptions.unshift('Все');
  }
} 