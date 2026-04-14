import {api} from "@/api/http.ts";

export const externalCalendarAccountApi = {
    getExternalCalendarAccounts: () => api.get("/oauth/accounts"),
    getAuthorizeUrl: () => api.get("/oauth/yandex/connect"),
    deleteExternalCalendarAccount: (externalCalendarAccountId: string) => api.delete(`/oauth/accounts/${externalCalendarAccountId}`),
};