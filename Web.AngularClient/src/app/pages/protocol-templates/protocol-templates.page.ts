import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { ProtocolTemplatesService } from '../../services/protocol-templates.service';
import { ProtocolTemplateClient, ProtocolTemplateDTO, ProtocolGroup, VerificationGroup, PossibleTemplateVerificationMethodsDTO, ServiceResult } from '../../api-client';

@Component({
  selector: 'app-protocol-templates-page',
  standalone: true,
  templateUrl: './protocol-templates.page.html',
  styleUrls: ['./protocol-templates.page.scss'],
  imports: [FormsModule, NgFor, NgIf],
  providers: [ProtocolTemplateClient],
})
export class ProtocolTemplatesPage implements OnInit {
  private readonly protocolTemplatesService = inject(ProtocolTemplatesService);
  private readonly protocolTemplateClient = inject(ProtocolTemplateClient);

  public protocolTemplates: ProtocolTemplateDTO[] = [];
  public possibleMethods: PossibleTemplateVerificationMethodsDTO[] = [];
  public loading = false;
  public loadingMethods = false;
  public error: string | null = null;
  public methodsError: string | null = null;
  public result: ServiceResult | null = null;
  public methodsResult: ServiceResult | null = null;
  
  public newTemplate = {
    verificationGroup: '' as string,
    protocolGroup: '' as string,
  };
  public addingTemplate = false;

  public verificationGroups = Object.values(VerificationGroup);
  public protocolGroups = Object.values(ProtocolGroup);

  ngOnInit(): void {
    this.protocolTemplatesService.setPageChangeCallback(() => this.loadTemplates());
    this.protocolTemplatesService.setPossibleMethodsPageChangeCallback(() => this.loadPossibleMethods());
    this.loadTemplates();
    this.loadPossibleMethods();
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
        this.error = msg || 'Не удалось загрузить шаблоны протоколов';
        this.loading = false;
      },
    });
  }

  public loadPossibleMethods(): void {
    this.loadingMethods = true;
    this.methodsError = null;
    this.protocolTemplateClient.getPossibleVerificationMethods(
      this.possibleMethodsPagination.currentPage,
      this.possibleMethodsPagination.pageSize
    ).subscribe({
      next: (result) => {
        if (result.data) {
          this.possibleMethods = result.data.items ?? [];
          this.protocolTemplatesService.updatePossibleMethodsPaginationFromData(result.data);
        } else {
          this.possibleMethods = [];
          this.protocolTemplatesService.resetPossibleMethodsPagination();
        }
        this.loadingMethods = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.methodsError = msg || 'Не удалось загрузить возможные методы поверки';
        this.loadingMethods = false;
      },
    });
  }

  public addTemplate(): void {
    if (!this.newTemplate.verificationGroup || !this.newTemplate.protocolGroup) {
      this.error = 'Нужно заполнить все поля';
      return;
    }
    this.addingTemplate = true;
    this.error = null;
    this.result = null;
    this.protocolTemplateClient.addTemplate(
      this.newTemplate.verificationGroup,
      this.newTemplate.protocolGroup
    ).subscribe({
      next: (result) => {
        this.newTemplate.verificationGroup = '';
        this.newTemplate.protocolGroup = '';
        this.addingTemplate = false;
        this.result = result;
        setTimeout(() => this.result = null, 5000);
        this.loadTemplates();
        this.loadPossibleMethods();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось добавить шаблон протокола';
        this.addingTemplate = false;
      },
    });
  }

  public onTemplateSelectChange(event: Event, verificationMethodId: string): void {
    const select = event.target as HTMLSelectElement;
    const templateId = select.value;
    if (templateId && verificationMethodId) {
      this.addVerificationMethodToTemplate(templateId, verificationMethodId);
      select.value = '';
    }
  }

  public addVerificationMethodToTemplate(templateId: string, verificationMethodId: string): void {
    this.protocolTemplateClient.addVerificationMethod(templateId, verificationMethodId).subscribe({
      next: (result) => {
        this.methodsResult = result;
        setTimeout(() => this.methodsResult = null, 5000);
        this.loadTemplates();
        this.loadPossibleMethods();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.methodsError = 'Ошибка добавления метода поверки: ' + (msg || 'Неизвестная ошибка');
        setTimeout(() => this.methodsError = null, 5000);
      },
    });
  }

  public deleteTemplate(id: string | undefined): void {
    if (!id) return;
    this.loading = true;
    this.error = null;
    this.result = null;
    this.protocolTemplateClient.deleteTemplate(id).subscribe({
      next: (result) => {
        this.result = result;
        setTimeout(() => this.result = null, 5000);
        this.loadTemplates();
        this.loadPossibleMethods();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось удалить шаблон протокола';
        this.loading = false;
      },
    });
  }

  public get pagination() {
    return this.protocolTemplatesService.getPagination();
  }

  public get possibleMethodsPagination() {
    return this.protocolTemplatesService.getPossibleMethodsPagination();
  }

  public getVerificationGroupDisplayName(group: VerificationGroup): string {
    return group.replace(/_/g, ' ');
  }

  public getProtocolGroupDisplayName(group: ProtocolGroup): string {
    return group;
  }

  public isMethodAlreadyInTemplate(template: ProtocolTemplateDTO, methodId: string): boolean {
    return template.verificationMethodsAliases?.includes(methodId) || false;
  }
} 