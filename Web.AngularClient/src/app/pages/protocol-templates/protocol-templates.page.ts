import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProtocolTemplatesService } from '../../services/protocol-templates.service';

// Temporary placeholder interface since ProtocolTemplateClient doesn't exist in new API
interface ProtocolTemplateDTO {
  id: string;
  name: string;
  description?: string;
}

@Component({
  selector: 'app-protocol-templates-page',
  standalone: true,
  templateUrl: './protocol-templates.page.html',
  styleUrls: ['./protocol-templates.page.scss'],
  imports: [FormsModule],
})
export class ProtocolTemplatesPage implements OnInit {
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
    
    // Temporary placeholder - API client doesn't include ProtocolTemplateClient
    setTimeout(() => {
      this.protocolTemplates = [];
      this.error = 'Функциональность шаблонов протоколов временно недоступна';
      this.loading = false;
    }, 500);
  }

  public deleteTemplate(id: string | undefined): void {
    if (!id) return;
    this.loading = true;
    
    // Temporary placeholder
    setTimeout(() => {
      this.error = 'Функциональность удаления временно недоступна';
      this.loading = false;
    }, 500);
  }

  public get pagination() {
    return this.protocolTemplatesService.getPagination();
  }
} 