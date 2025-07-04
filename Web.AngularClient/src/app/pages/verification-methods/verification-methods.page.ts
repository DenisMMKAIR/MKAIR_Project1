import { Component, inject } from '@angular/core';
import { VerificationMethodsService } from '../../services/verification-methods.service';

@Component({
  selector: 'app-verification-methods-page',
  standalone: true,
  templateUrl: './verification-methods.page.html',
  styleUrls: ['./verification-methods.page.scss']
})
export class VerificationMethodsPage {
  private readonly verificationMethodsService = inject(VerificationMethodsService);
} 