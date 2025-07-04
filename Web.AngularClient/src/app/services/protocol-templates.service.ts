import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';

@Injectable({
  providedIn: 'root'
})
export class ProtocolTemplatesService {
  private pagination: Pagination;

  constructor() {
    this.pagination = new Pagination();
  }

  public setPageChangeCallback(callback: () => void): void {
    this.pagination.setPageChangeCallback(callback);
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
} 