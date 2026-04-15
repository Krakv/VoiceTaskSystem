import { useEffect, useState } from "react";
import { accountApi } from "@/api/account.api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import type { Account } from "@/types/account";
import {toast} from "sonner";
import type {ExternalCalendarAccount} from "@/types/externalCalendarAccount.ts";
import {externalCalendarAccountApi} from "@/api/externalCalendarAccount.api.ts";

export const AccountForm = () => {
    const [account, setAccount] = useState<Account | null>(null);
    const [loading, setLoading] = useState(false);

    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [calendars, setCalendars] = useState<ExternalCalendarAccount[]>([]);

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");

    useEffect(() => {
        load();
        loadCalendars();
    }, []);

    const loadCalendars = async () => {
        const res = await externalCalendarAccountApi.getExternalCalendarAccounts();
        setCalendars(res.data.data);
    };

    const load = async () => {
        const res = await accountApi.getAccount();
        const data = res.data.data;

        setAccount(data);
        setName(data.name);
        setEmail(data.email || "");
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
            await accountApi.changePassword({
                currentPassword,
                newPassword,
            });

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

    return (
        <div className="max-w-md mx-auto p-3 space-y-8 pb-16">

            <h2 className="text-xl font-bold">Личный кабинет</h2>

            {/* INFO BLOCK */}
            {account && (
                <div className="space-y-3 border p-3 rounded-xl">
                    <div>
                        <Label>Имя</Label>
                        <div>{account.name}</div>
                    </div>

                    <div>
                        <Label>Email</Label>
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
                                onClick={sendEmailVerification}
                            >
                                Подтвердить email
                            </Button>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label>Telegram</Label>

                        <div className="text-sm">
                            {account?.telegramChatId
                                ? "✅ привязан"
                                : "❌ не привязан"}
                        </div>

                        <div className="flex gap-2">
                            {!account?.telegramChatId && (
                                <Button
                                    size="sm"
                                    onClick={connectTelegram}
                                >
                                    Подключить Telegram
                                </Button>
                            )}

                            {account?.telegramChatId && (
                                <Button
                                    size="sm"
                                    variant="destructive"
                                    onClick={unlinkTelegram}
                                >
                                    Отвязать
                                </Button>
                            )}
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label>Календари</Label>

                        {calendars.length === 0 ? (
                            <div className="text-sm text-muted-foreground">
                                Нет подключённых календарей
                            </div>
                        ) : (
                            calendars.map((c) => (
                                <div
                                    key={c.externalCalendarAccountId}
                                    className="flex items-center justify-between border rounded-lg p-2"
                                >
                                    <div>
                                        <div className="text-sm font-medium truncate max-w-[200px]">
                                            {c.baseUrl}
                                        </div>
                                    </div>

                                    <Button
                                        size="sm"
                                        variant="destructive"
                                        onClick={() =>
                                            unlinkCalendar(c.externalCalendarAccountId)
                                        }
                                    >
                                        Отвязать
                                    </Button>
                                </div>
                            ))
                        )}

                        <Button variant="outline" onClick={connectYandexCalendar}>
                            + Подключить Яндекс календарь
                        </Button>
                    </div>
                </div>
            )}

            {/* UPDATE PROFILE */}
            <div className="space-y-2">
                <Label>Имя</Label>
                <Input value={name} onChange={e => setName(e.target.value)} />

                <Label>Email</Label>
                <Input value={email} onChange={e => setEmail(e.target.value)} />

                <Button onClick={updateProfile} disabled={loading}>
                    Обновить профиль
                </Button>
            </div>

            {/* PASSWORD */}
            <div className="space-y-2 pt-4">
                <Label>Текущий пароль</Label>
                <Input
                    type="password"
                    value={currentPassword}
                    onChange={e => setCurrentPassword(e.target.value)}
                />

                <Label>Новый пароль</Label>
                <Input
                    type="password"
                    value={newPassword}
                    onChange={e => setNewPassword(e.target.value)}
                />

                <Button onClick={changePassword}>
                    Сменить пароль
                </Button>
            </div>
        </div>
    );
};