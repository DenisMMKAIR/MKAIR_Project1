export interface IPaginationData {
  pageIndex?: number;
  totalCount?: number;
  totalPages?: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

export class Pagination {
  private onPageChangeCallback?: () => void;

  public currentPage = 1;
  public pageSize = 10;
  public totalCount = 0;
  public totalPages = 0;
  public hasPreviousPage = false;
  public hasNextPage = false;
  public pageSizeOptions = [5, 10, 100];

  constructor(onPageChange?: () => void) {
    this.onPageChangeCallback = onPageChange;
  }

  public updateFromData(data: IPaginationData): void {
    this.totalCount = data.totalCount ?? 0;
    this.totalPages = data.totalPages ?? 0;
    this.hasPreviousPage = data.hasPreviousPage ?? false;
    this.hasNextPage = data.hasNextPage ?? false;
    
    const requestedPage = data.pageIndex ?? 1;
    this.currentPage = this.isValidPage(requestedPage) ? requestedPage : 1;
  }

  public reset(): void {
    this.currentPage = 1;
    this.totalCount = 0;
    this.totalPages = 0;
    this.hasPreviousPage = false;
    this.hasNextPage = false;
  }

  public onPageSizeChange(): void {
    this.resetToFirstPage();
  }

  public goToPage(page: number): void {
    if (this.isValidPage(page)) {
      this.currentPage = page;
      this.triggerPageChange();
    }
  }

  public goToFirstPage(): void {
    if (this.canActivateFirst) {
      this.goToPage(1);
    }
  }

  public goToLastPage(): void {
    if (this.canActivateLast) {
      this.goToPage(this.totalPages);
    }
  }

  public goToPreviousPage(): void {
    if (this.hasPreviousPage) {
      this.goToPage(this.currentPage - 1);
    }
  }

  public goToNextPage(): void {
    if (this.hasNextPage) {
      this.goToPage(this.currentPage + 1);
    }
  }

  public resetToFirstPage(): void {
    this.currentPage = 1;
    this.triggerPageChange();
  }

  public isValidPage(page: number): boolean {
    return page >= 1 && page <= this.totalPages;
  }

  public getStartIndex(): number {
    return (this.currentPage - 1) * this.pageSize;
  }

  public getEndIndex(): number {
    if (this.totalCount === 0) {
      return -1;
    }
    return Math.min(this.getStartIndex() + this.pageSize - 1, this.totalCount - 1);
  }

  public hasItems(): boolean {
    return this.totalCount > 0;
  }

  public needsPagination(): boolean {
    return this.totalPages > 1;
  }

  public setPageChangeCallback(callback: () => void): void {
    this.onPageChangeCallback = callback;
  }

  public get canActivateFirst(): boolean {
    return this.totalPages > 0 && this.currentPage !== 1;
  }

  public get canActivateLast(): boolean {
    return this.totalPages > 0 && this.currentPage !== this.totalPages;
  }

  public getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisiblePages = 5;
    
    if (this.totalPages <= maxVisiblePages) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      let start = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
      let end = Math.min(this.totalPages, start + maxVisiblePages - 1);
      
      if (end - start < maxVisiblePages - 1) {
        start = Math.max(1, end - maxVisiblePages + 1);
      }
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    
    return pages;
  }

  private triggerPageChange(): void {
    if (this.onPageChangeCallback) {
      this.onPageChangeCallback();
    }
  }
}
