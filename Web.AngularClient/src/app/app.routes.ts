import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'jobs',
    loadComponent: () =>
      import('./pages/jobs/jobs.page').then((m) => m.JobsPage),
  },
  {
    path: 'verifications',
    loadComponent: () =>
      import('./pages/verifications/verifications.page').then(
        (m) => m.VerificationsPage
      ),
  },
  {
    path: 'verification-methods',
    loadComponent: () =>
      import('./pages/verification-methods/verification-methods.page').then(
        (m) => m.VerificationMethodsPage
      ),
  },
  {
    path: 'protocol-templates',
    loadComponent: () =>
      import('./pages/protocol-templates/protocol-templates.page').then(
        (m) => m.ProtocolTemplatesPage
      ),
  },
];
