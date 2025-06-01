import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { BrowserModule } from '@angular/platform-browser';
import { MsalInterceptor, MsalModule } from '@azure/msal-angular';
import { msalGuardConfig, msalInstance, msalInterceptorConfig } from './identity/msal/msalConfiguration';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    importProvidersFrom(
      BrowserModule,
      MsalModule.forRoot(msalInstance,msalGuardConfig, msalInterceptorConfig)
    )
  ]
};
