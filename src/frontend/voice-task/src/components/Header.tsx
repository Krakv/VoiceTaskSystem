import { useAuthStore } from '@/stores/authStore';
import {useLocation, useNavigate} from "react-router-dom";
import {Button} from "@/components/ui/button.tsx";
import {ArrowLeft, LogOut} from "lucide-react";

const Header = () => {
    const logout = useAuthStore((state) => state.logout);
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    const handleBack = () => {
        navigate(-1);
    };

    const hideBackButton = location.pathname === "/tasks";

    return (
        <header className="flex justify-between items-right p-4 bg-white border-b border-gray-200">
            {!hideBackButton && (
                <Button
                    variant="ghost"
                    className="p-2"
                    onClick={handleBack}
                    title="Назад"
                >
                    <ArrowLeft className="w-5 h-5" />
                </Button>
            )}

            {!hideBackButton && <div className="flex-1" />}

            <Button
                variant="ghost"
                className="p-2"
                onClick={handleLogout}
                title="Выйти"
            >
                <LogOut className="w-5 h-5" />
            </Button>
        </header>
    );
};

export default Header;