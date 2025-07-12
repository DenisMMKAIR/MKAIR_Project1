import { Injectable } from '@angular/core';
import { Pagination, IPaginationData } from '../shared/pagination';

type TableKey = 'verification' | 'possible';

interface AddVerificationMethodFormData {
  description: string;
  aliasInput: string;
  checkupKey: string;
  checkupValue: string;
}

@Injectable({ providedIn: 'root' })
export class VerificationMethodsService {
  private paginations: Record<TableKey, Pagination> = {
    verification: new Pagination(),
    possible: new Pagination(),
  };
  private filters = {
    verification: {
      verificationNameFilter: null as string | null,
      deviceTypeInfoFilter: null as string | null,
      yearMonthFilter: '' as string,
      deviceTypeNumberFilter: '' as string,
    },
    possible: {
      verificationNameFilter: null as string | null,
      deviceTypeInfoFilter: null as string | null,
      yearMonthFilter: '' as string,
      deviceTypeNumberFilter: '' as string,
    },
  };

  private addVerificationMethodFormData: AddVerificationMethodFormData = {
    description: '',
    aliasInput: '',
    checkupKey: '',
    checkupValue: '',
  };

  public setPageChangeCallback(key: TableKey, callback: () => void): void {
    this.paginations[key].setPageChangeCallback(callback);
  }

  public getPagination(key: TableKey): Pagination {
    return this.paginations[key];
  }

  public updatePaginationFromData(key: TableKey, data: IPaginationData): void {
    this.paginations[key].updateFromData(data);
  }

  public resetPagination(key: TableKey): void {
    this.paginations[key].reset();
    this.filters[key].yearMonthFilter = '';
  }

  public resetToFirstPage(key: TableKey): void {
    this.paginations[key].resetToFirstPage();
  }

  public getVerificationNameFilter(key: TableKey): string | null {
    return this.filters[key].verificationNameFilter;
  }
  public setVerificationNameFilter(key: TableKey, value: string | null): void {
    this.filters[key].verificationNameFilter = value;
  }
  public getDeviceTypeInfoFilter(key: TableKey): string | null {
    return this.filters[key].deviceTypeInfoFilter;
  }
  public setDeviceTypeInfoFilter(key: TableKey, value: string | null): void {
    this.filters[key].deviceTypeInfoFilter = value;
  }
  public getYearMonthFilter(key: TableKey): string {
    const v = this.filters[key].yearMonthFilter;
    return !v || v === 'all' ? '' : v;
  }
  public setYearMonthFilter(key: TableKey, value: string | null): void {
    this.filters[key].yearMonthFilter = !value || value === 'all' ? '' : value;
  }
  public getDeviceTypeNumberFilter(key: TableKey): string {
    return this.filters[key].deviceTypeNumberFilter;
  }
  public setDeviceTypeNumberFilter(key: TableKey, value: string): void {
    this.filters[key].deviceTypeNumberFilter = value;
  }
  public resetFilters(key: TableKey): void {
    this.filters[key].verificationNameFilter = null;
    this.filters[key].deviceTypeInfoFilter = null;
    this.filters[key].yearMonthFilter = '';
    this.filters[key].deviceTypeNumberFilter = '';
  }

  // Add verification method form data persistence
  public getAddVerificationMethodFormData(): AddVerificationMethodFormData {
    return { ...this.addVerificationMethodFormData };
  }

  public setAddVerificationMethodFormData(data: Partial<AddVerificationMethodFormData>): void {
    this.addVerificationMethodFormData = { ...this.addVerificationMethodFormData, ...data };
  }

  public clearAddVerificationMethodFormData(): void {
    this.addVerificationMethodFormData = {
      description: '',
      aliasInput: '',
      checkupKey: '',
      checkupValue: '',
    };
  }
} 