import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { OwnersClient, Owner } from '../../api-client';
import { FormsModule } from '@angular/forms';
import { OwnersService } from '../../services/owners.service';

@Component({
  selector: 'app-owners-page',
  standalone: true,
  templateUrl: './owners.page.html',
  styleUrls: ['./owners.page.scss'],
  imports: [NgFor, NgIf, FormsModule],
  providers: [OwnersClient],
})
export class OwnersPage implements OnInit {
  private readonly ownersClient = inject(OwnersClient);
  private readonly ownersService = inject(OwnersService);

  public owners: Owner[] = [];
  public loading = false;
  public error: string | null = null;

  public newOwnerName: string = '';
  public newOwnerINN: number | null = null;
  public addLoading = false;
  public addError: string | null = null;

  public editOwnerId: string | null = null;
  public editOwnerINN: number | null = null;
  public editLoading = false;
  public editError: string | null = null;

  ngOnInit(): void {
    this.ownersService.setPageChangeCallback(() => this.loadOwners());
    this.loadOwners();
  }

  public loadOwners(): void {
    this.loading = true;
    this.error = null;
    this.ownersClient.getOwners(this.pagination.currentPage, this.pagination.pageSize).subscribe({
      next: (result) => {
        if (result.data) {
          this.owners = result.data.items ?? [];
          this.ownersService.updatePaginationFromData(result.data);
        } else {
          this.owners = [];
          this.ownersService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить владельцев';
        this.loading = false;
      },
    });
  }

  public addOwner(): void {
    if (!this.newOwnerName || this.newOwnerINN == null || isNaN(this.newOwnerINN)) return;
    this.addLoading = true;
    this.addError = null;
    const inn = Math.floor(Math.abs(this.newOwnerINN));
    this.ownersClient.addOwner(this.newOwnerName, inn).subscribe({
      next: (res) => {
        if (res.error) {
          this.addError = res.error;
          this.addLoading = false;
          return;
        }
        this.addLoading = false;
        this.newOwnerName = '';
        this.newOwnerINN = null;
        this.loadOwners();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.addError = msg || 'Не удалось добавить владельца';
        this.addLoading = false;
      },
    });
  }

  public startEditOwnerINN(owner: Owner): void {
    this.editOwnerId = owner.id || null;
    this.editOwnerINN = owner.inn || null;
    this.editError = null;
  }

  public saveEditOwnerINN(owner: Owner): void {
    if (!this.editOwnerId || !this.editOwnerINN) return;
    this.editLoading = true;
    this.editError = null;
    this.ownersClient.setOwnerINN(this.editOwnerId, this.editOwnerINN).subscribe({
      next: (res) => {
        if (res.error) {
          this.editError = res.error;
          this.editLoading = false;
          return;
        }
        this.editLoading = false;
        this.editOwnerId = null;
        this.editOwnerINN = null;
        this.loadOwners();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.editError = msg || 'Не удалось изменить ИНН';
        this.editLoading = false;
      },
    });
  }

  public cancelEditOwnerINN(): void {
    this.editOwnerId = null;
    this.editOwnerINN = null;
    this.editError = null;
  }

  public get pagination() {
    return this.ownersService.getPagination();
  }
} 