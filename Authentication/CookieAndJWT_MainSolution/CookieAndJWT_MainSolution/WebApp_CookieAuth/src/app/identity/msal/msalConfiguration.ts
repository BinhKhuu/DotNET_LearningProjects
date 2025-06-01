import { MsalGuardConfiguration, MsalInterceptorConfiguration } from '@azure/msal-angular';
import { BrowserCacheLocation, InteractionType, PublicClientApplication } from '@azure/msal-browser';
export const msalGuardConfig: MsalGuardConfiguration = {
    interactionType: InteractionType.Redirect,
};
export const msalInstance = new PublicClientApplication({
    auth: {
        clientId: 'cf4be252-2340-4a9f-bd46-3bd65df25846',
        authority: 'https://login.microsoftonline.com/fb2492a7-4cfe-4506-95f9-2284aa380e41',
        redirectUri: 'https://localhost:7198/auth-redirect',
        postLogoutRedirectUri: 'https://localhost:7198'
    },
    cache: {
        cacheLocation: BrowserCacheLocation.SessionStorage,
        storeAuthStateInCookie: false,
    },
    system: {
        loggerOptions: {
            loggerCallback: () => {},
            piiLoggingEnabled: false,
        },
    },
});

export const msalInterceptorConfig: MsalInterceptorConfiguration = {
    interactionType: InteractionType.Redirect,
    protectedResourceMap: new Map([
        ['https://localhost:7198', ['api://cf4be252-2340-4a9f-bd46-3bd65df25846/access_as_user']]
    ])
}