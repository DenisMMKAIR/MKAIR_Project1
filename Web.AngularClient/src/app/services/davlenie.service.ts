import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

export interface IDavlenieFilter {
  yearMonthFilter: string | null;
  deviceTypeNumberFilter: string | null;
  deviceSerialFilter: string | null;
  locationFilter: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class DavlenieService {
  private currentFilters: IDavlenieFilter = {
    yearMonthFilter: '',
    deviceTypeNumberFilter: null,
    deviceSerialFilter: null,
    locationFilter: 'all'
  };

  private pagination: Pagination;
  private yearMonthOptions: string[] = [];

  private deviceTypeNumberFilterSubject = new Subject<string | null>();
  private deviceSerialFilterSubject = new Subject<string | null>();

  constructor() {
    this.pagination = new Pagination();
    this.pagination.pageSize = 5;
    this.generateYearMonthOptions();

    this.deviceTypeNumberFilterSubject
      .pipe(
        debounceTime(1000),
        distinctUntilChanged()
      )
      .subscribe(filter => {
        this.currentFilters.deviceTypeNumberFilter = filter;
        this.resetToFirstPage();
      });

    this.deviceSerialFilterSubject
      .pipe(
        debounceTime(1000),
        distinctUntilChanged()
      )
      .subscribe(filter => {
        this.currentFilters.deviceSerialFilter = filter;
        this.resetToFirstPage();
      });
  }

  public setPageChangeCallback(callback: () => void): void {
    this.pagination.setPageChangeCallback(callback);
  }

  public getYearMonthOptions(): string[] {
    return [...this.yearMonthOptions];
  }

  public getYearMonthFilter(): string {
    return !this.currentFilters.yearMonthFilter || this.currentFilters.yearMonthFilter === 'all' ? '' : this.currentFilters.yearMonthFilter;
  }

  public setYearMonthFilter(filter: string | null): void {
    this.currentFilters.yearMonthFilter = !filter || filter === 'all' ? '' : filter;
  }

  public getDeviceTypeNumberFilter(): string | null {
    return this.currentFilters.deviceTypeNumberFilter;
  }

  public setDeviceTypeNumberFilter(filter: string | null): void {
    this.deviceTypeNumberFilterSubject.next(filter);
  }

  public getDeviceSerialFilter(): string | null {
    return this.currentFilters.deviceSerialFilter;
  }

  public setDeviceSerialFilter(filter: string | null): void {
    this.deviceSerialFilterSubject.next(filter);
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
    this.currentFilters.yearMonthFilter = '';
    this.currentFilters.deviceTypeNumberFilter = null;
    this.currentFilters.deviceSerialFilter = null;
    this.currentFilters.locationFilter = 'all';
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