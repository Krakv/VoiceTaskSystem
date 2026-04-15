
export interface Account {
    id: string;
    name: string;
    email?: string;
    emailVerified?: boolean;
    externalCalendarAccountIds?: string[];
    telegramChatId?: string;
}