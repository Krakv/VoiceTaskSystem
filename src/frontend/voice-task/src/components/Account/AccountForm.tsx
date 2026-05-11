import { useEffect, useState } from "react";
import { accountApi } from "@/api/account.api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { Loader2 } from "lucide-react";

import type { Account } from "@/types/account";
import { toast } from "sonner";
import type { ExternalCalendarAccount } from "@/types/externalCalendarAccount.ts";
import { externalCalendarAccountApi } from "@/api/externalCalendarAccount.api.ts";

export const AccountForm = () => {
    const [account, setAccount] = useState<Account | null>(null);
    const [initialLoading, setInitialLoading] = useState(true);
    const [loading, setLoading] = useState(false);

    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [calendars, setCalendars] = useState<ExternalCalendarAccount[]>([]);

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");

    useEffect(() => {
        Promise.all([load(), loadCalendars()]).finally(() => {
            setInitialLoading(false);
        });
    }, []);

    const loadCalendars = async () => {
        try {
            const res = await externalCalendarAccountApi.getExternalCalendarAccounts();
            setCalendars(res.data.data);
        } catch (err) {
            console.error("Ошибка загрузки календарей", err);
        }
    };

    const load = async () => {
        try {
            const res = await accountApi.getAccount();
            const data = res.data.data;

            setAccount(data);
            setName(data.name);
            setEmail(data.email || "");
        } catch {
            toast.error("Ошибка загрузки профиля");
        }
    };

    const connectTelegram = async () => {
        try {
            const res = await accountApi.getTelegramLinkToken();
            const token = res.data.data;
            const url = `https://t.me/SpeakTaskBot?start=${token}`;
            window.open(url, "_blank");
        } catch {
            toast.error("Ошибка получения токена Telegram");
        }
    };

    const updateProfile = async () => {
        setLoading(true);
        try {
            await accountApi.updateAccount({ name, email });
            await load();
            toast.info("Обновлено");
        } catch {
            toast.error("Ошибка обновления профиля");
        } finally {
            setLoading(false);
        }
    };

    const changePassword = async () => {
        try {
            await accountApi.changePassword({ currentPassword, newPassword });
            setCurrentPassword("");
            setNewPassword("");
            toast.info("Пароль обновлён");
        } catch {
            toast.error("Ошибка смены пароля");
        }
    };

    const sendEmailVerification = async () => {
        try {
            await accountApi.sendEmailVerification();
            toast.info("Письмо отправлено");
        } catch {
            toast.error("Ошибка отправки письма");
        }
    };

    const unlinkTelegram = async () => {
        try {
            await accountApi.unlinkTelegram();
            await load();
        } catch {
            toast.error("Ошибка отвязки Telegram");
        }
    };

    const connectYandexCalendar = async () => {
        const res = await externalCalendarAccountApi.getAuthorizeUrl();
        window.location.href = res.data.data;
    };

    const unlinkCalendar = async (id: string) => {
        try {
            await externalCalendarAccountApi.deleteExternalCalendarAccount(id);
            await loadCalendars();
        } catch {
            toast.error("Ошибка отвязки календаря");
        }
    };

    if (initialLoading) {
        return (
            <div className="max-w-md mx-auto p-4 space-y-6">
                <Skeleton className="h-8 w-48" />
                <Skeleton className="h-64 w-full rounded-xl" />
                <div className="space-y-2">
                    <Skeleton className="h-4 w-20" />
                    <Skeleton className="h-10 w-full" />
                </div>
            </div>
        );
    }

    return (
        <div className="max-w-md mx-auto p-3 space-y-8 pb-16">
            <h2 className="text-xl font-bold">Личный кабинет</h2>

            {/* INFO BLOCK */}
            {account && (
                <div className="space-y-3 border p-3 rounded-xl bg-card">
                    <div>
                        <Label className="text-muted-foreground">Имя</Label>
                        <div className="font-medium">{account.name}</div>
                    </div>

                    <div>
                        <Label className="text-muted-foreground">Email</Label>
                        <div>{account.email || "не указан"}</div>

                        {account.email && (
                            <div className="text-sm mt-1">
                                {account.emailVerified
                                    ? "✅ подтверждён"
                                    : "❌ не подтверждён"}
                            </div>
                        )}

                        {!account.emailVerified && account.email && (
                            <Button
                                size="sm"
                                variant="outline"
                                className="mt-2"
                                onClick={sendEmailVerification}
                            >
                                Подтвердить email
                            </Button>
                        )}
                    </div>

                    <div className="space-y-2 border-t pt-2">
                        <Label className="text-muted-foreground">Telegram</Label>
                        <div className="text-sm">
                            {account?.telegramChatId
                                ? "✅ привязан"
                                : "❌ не привязан"}
                        </div>
                        <div className="flex gap-2">
                            {!account?.telegramChatId ? (
                                <Button size="sm" onClick={connectTelegram}>
                                    Подключить Telegram
                                </Button>
                            ) : (
                                <Button size="sm" variant="destructive" onClick={unlinkTelegram}>
                                    Отвязать
                                </Button>
                            )}
                        </div>
                    </div>

                    <div className="space-y-2 border-t pt-2">
                        <Label className="text-muted-foreground">Календари</Label>
                        {calendars.length === 0 ? (
                            <div className="text-sm text-muted-foreground italic">
                                Нет подключённых календарей
                            </div>
                        ) : (
                            calendars.map((c) => (
                                <div
                                    key={c.externalCalendarAccountId}
                                    className="flex items-center justify-between border rounded-lg p-2 bg-background"
                                >
                                    <div className="text-sm font-medium truncate max-w-[180px]">
                                        {c.baseUrl}
                                    </div>
                                    <Button
                                        size="xs"
                                        variant="ghost"
                                        className="text-destructive"
                                        onClick={() => unlinkCalendar(c.externalCalendarAccountId)}
                                    >
                                        Отвязать
                                    </Button>
                                </div>
                            ))
                        )}
                        <Button variant="outline" className="w-full text-xs" onClick={connectYandexCalendar}>
                            + Подключить Яндекс календарь
                        </Button>
                    </div>
                </div>
            )}

            {/* UPDATE PROFILE */}
            <div className="space-y-4 border-t pt-4">
                <h3 className="font-semibold text-sm">Настройки профиля</h3>
                <div className="space-y-2">
                    <Label>Имя</Label>
                    <Input value={name} onChange={e => setName(e.target.value)} />
                </div>
                <div className="space-y-2">
                    <Label>Email</Label>
                    <Input value={email} onChange={e => setEmail(e.target.value)} />
                </div>
                <Button onClick={updateProfile} disabled={loading} className="w-full">
                    {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                    Обновить профиль
                </Button>
            </div>

            {/* PASSWORD */}
            <div className="space-y-4 border-t pt-4">
                <h3 className="font-semibold text-sm">Безопасность</h3>
                <div className="space-y-2">
                    <Label>Текущий пароль</Label>
                    <Input
                        type="password"
                        value={currentPassword}
                        onChange={e => setCurrentPassword(e.target.value)}
                    />
                </div>
                <div className="space-y-2">
                    <Label>Новый пароль</Label>
                    <Input
                        type="password"
                        value={newPassword}
                        onChange={e => setNewPassword(e.target.value)}
                    />
                </div>
                <Button onClick={changePassword} variant="secondary" className="w-full">
                    Сменить пароль
                </Button>
            </div>
        </div>
    );
};