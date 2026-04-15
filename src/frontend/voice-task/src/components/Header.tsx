import { useAuthStore } from '@/stores/authStore';
import { useLocation, useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { ArrowLeft, LogOut, User } from "lucide-react";

const routeTitles: Record<string, string> = {
    "/tasks": "Задачи",
    "/create": "Создание задачи",
    "/create/voice": "Голосовой ввод",
    "/notifications": "Уведомления",
    "/rules": "Правила",
    "/calendar": "Календарь",
    "/account": "Аккаунт",
};

const Header = () => {
    const logout = useAuthStore((state) => state.logout);
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const matchedKey = Object.keys(routeTitles).find((p) =>
        location.pathname.startsWith(p)
    );

    const title =
        routeTitles[location.pathname] ||
        (matchedKey ? routeTitles[matchedKey] : "") ||
        "";

    const hideBackButton = location.pathname === "/tasks" || location.pathname === "/notifications" || location.pathname === "/rules" || location.pathname === "/calendar";

    return (
        <header className="flex items-center justify-between px-5 py-4 border-b bg-background">
            <div className="flex items-center gap-3">
                {!hideBackButton && (
                    <Button
                        variant="outline"
                        size="icon"
                        className="h-10 w-10"
                        onClick={() => navigate(-1)}
                        title="Назад"
                    >
                        <ArrowLeft className="w-5 h-5" />
                    </Button>
                )}

                <h1 className="text-lg font-semibold tracking-tight">
                    {title}
                </h1>
            </div>

            {/* RIGHT */}
            <div className="flex items-center gap-2">
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-10 w-10"
                    onClick={() => navigate("/account")}
                    title="Аккаунт"
                >
                    <User className="w-5 h-5" />
                </Button>

                <Button
                    variant="ghost"
                    size="icon"
                    className="h-10 w-10"
                    onClick={handleLogout}
                    title="Выйти"
                >
                    <LogOut className="w-5 h-5" />
                </Button>
            </div>
        </header>
    );
};

export default Header;