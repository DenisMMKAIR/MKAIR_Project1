import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, KeyValuePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProtocolTemplateClient, ProtocolTemplateDTO } from '../../api-client';
import { ProtocolTemplatesService } from '../../services/protocol-templates.service';

@Component({
  selector: 'app-protocol-templates-page',
  standalone: true,
  templateUrl: './protocol-templates.page.html',
  styleUrls: ['./protocol-templates.page.scss'],
  imports: [NgFor, NgIf, FormsModule, KeyValuePipe],
  providers: [ProtocolTemplateClient],
})
export class ProtocolTemplatesPage implements OnInit {
  private readonly protocolTemplateClient = inject(ProtocolTemplateClient);
  private readonly protocolTemplatesService = inject(ProtocolTemplatesService);

  public protocolTemplates: ProtocolTemplateDTO[] = [];
  public loading = false;
  public error: string | null = null;

  ngOnInit(): void {
    this.protocolTemplatesService.setPageChangeCallback(() => this.loadTemplates());
    this.loadTemplates();
  }

  public loadTemplates(): void {
    this.loading = true;
    this.error = null;
    this.protocolTemplateClient.getTemplates(
      this.pagination.currentPage,
      this.pagination.pageSize
    ).subscribe({
      next: (result) => {
        if (result.data) {
          this.protocolTemplates = result.data.items ?? [];
          this.protocolTemplatesService.updatePaginationFromData(result.data);
        } else {
          this.protocolTemplates = [];
          this.protocolTemplatesService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить шаблоны';
        this.loading = false;
      },
    });
  }

  public deleteTemplate(id: string | undefined): void {
    if (!id) return;
    this.loading = true;
    this.protocolTemplateClient.deleteTemplate(Number(id)).subscribe({
      next: () => this.loadTemplates(),
      error: (err) => {
        this.error = err?.error?.error || err?.error?.message || err?.message || 'Ошибка удаления';
        this.loading = false;
      },
    });
  }

  public get pagination() {
    return this.protocolTemplatesService.getPagination();
  }
} 