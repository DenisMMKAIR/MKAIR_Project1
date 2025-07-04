import { Component, inject } from '@angular/core';
import { ProtocolTemplatesService } from '../../services/protocol-templates.service';

@Component({
  selector: 'app-protocol-templates-page',
  standalone: true,
  templateUrl: './protocol-templates.page.html',
  styleUrls: ['./protocol-templates.page.scss']
})
export class ProtocolTemplatesPage {
  private readonly protocolTemplatesService = inject(ProtocolTemplatesService);
} 