import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';
import { ProtocolTemplateClient, ProtocolTemplateDTO, PossibleTemplateVerificationMethodsDTO } from '../api-client';

@Injectable({
  providedIn: 'root'
})
export class ProtocolTemplatesService {
  private pagination: Pagination;
  private possibleMethodsPagination: Pagination;

  constructor() {
    this.pagination = new Pagination();
    this.possibleMethodsPagination = new Pagination();
  }

  public setPageChangeCallback(callback: () => void): void {
    this.pagination.setPageChangeCallback(callback);
  }

  public setPossibleMethodsPageChangeCallback(callback: () => void): void {
    this.possibleMethodsPagination.setPageChangeCallback(callback);
  }

  public getPagination(): Pagination {
    return this.pagination;
  }

  public getPossibleMethodsPagination(): Pagination {
    return this.possibleMethodsPagination;
  }

  public updatePaginationFromData(data: IPaginationData): void {
    this.pagination.updateFromData(data);
  }

  public updatePossibleMethodsPaginationFromData(data: IPaginationData): void {
    this.possibleMethodsPagination.updateFromData(data);
  }

  public resetPagination(): void {
    this.pagination.reset();
  }

  public resetPossibleMethodsPagination(): void {
    this.possibleMethodsPagination.reset();
  }

  public resetToFirstPage(): void {
    this.pagination.resetToFirstPage();
  }

  public resetPossibleMethodsToFirstPage(): void {
    this.possibleMethodsPagination.resetToFirstPage();
  }
} 