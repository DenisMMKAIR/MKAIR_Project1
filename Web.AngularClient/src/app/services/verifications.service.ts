import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

export interface IVerificationsFilter {
  yearMonthFilter: string | null;
  typeInfoFilter: string | null;
  locationFilter: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class VerificationsService {
  private currentFilters: IVerificationsFilter = {
    yearMonthFilter: 'all',
    typeInfoFilter: null,
    locationFilter: 'all'
  };

  private pagination: Pagination;
  private yearMonthOptions: string[] = [];
  
  private typeInfoFilterSubject = new Subject<string | null>();

  constructor() {
    this.pagination = new Pagination();
    this.generateYearMonthOptions();
    
    this.typeInfoFilterSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(filter => {
        this.currentFilters.typeInfoFilter = filter;
        this.resetToFirstPage();
      });
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

  public getTypeInfoFilter(): string | null {
    return this.currentFilters.typeInfoFilter;
  }

  public setTypeInfoFilter(filter: string | null): void {
    this.typeInfoFilterSubject.next(filter);
  }

  public getLocationFilter(): string | null {
    return this.currentFilters.locationFilter;
  }

  public setLocationFilter(filter: string | null): void {
    this.currentFilters.locationFilter = filter;
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