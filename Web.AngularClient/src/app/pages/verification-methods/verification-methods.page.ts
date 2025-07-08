import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VerificationMethodsTableComponent } from './verification-methods-table/verification-methods-table.component';
import { PossibleVerificationMethodsTableComponent } from './possible-verification-methods-table/possible-verification-methods-table.component';
import { AddVerificationMethodComponent } from './add-verification-method/add-verification-method.component';

@Component({
  selector: 'app-verification-methods-page',
  standalone: true,
  templateUrl: './verification-methods.page.html',
  styleUrls: ['./verification-methods.page.scss'],
  imports: [CommonModule, AddVerificationMethodComponent, VerificationMethodsTableComponent, PossibleVerificationMethodsTableComponent],
})
export class VerificationMethodsPage {} 