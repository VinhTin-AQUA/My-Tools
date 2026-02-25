import { AppRouteNames, AuthRouteNames, MainRouteNames } from '../../core/enums/route-names';

export const RouteNavigationHelper = {
    AUTH: {
        root: [AppRouteNames.Auth],
        config: [AppRouteNames.Auth, AuthRouteNames.Config],
    },
    MAIN: {
        root: [AppRouteNames.Main],
        home: [AppRouteNames.Main, MainRouteNames.Home],
        invoices: [AppRouteNames.Main, MainRouteNames.Invoices],
        configDetails: [AppRouteNames.Main, MainRouteNames.ConfigDetails],
        settings: [AppRouteNames.Main, MainRouteNames.Settings],
        addInvoice: [AppRouteNames.Main, MainRouteNames.AddInvoice],
    },
};
