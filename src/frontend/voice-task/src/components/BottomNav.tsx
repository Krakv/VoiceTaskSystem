import { type ReactNode } from "react";
import { Home, Calendar, ClipboardList, Bell } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useLocation, useNavigate } from "react-router-dom";

interface NavItem {
    label: string;
    icon: ReactNode;
    path: string;
}

export const BottomNav = () => {
    const { pathname } = useLocation();
    const navigate = useNavigate();

    const navItems: NavItem[] = [
        { label: "Задачи", icon: <Home className="w-5 h-5" />, path: "/tasks" },
        { label: "Календарь", icon: <Calendar className="w-5 h-5" />, path: "/calendar" },
        { label: "Правила", icon: <ClipboardList className="w-5 h-5" />, path: "/rules" },
        { label: "Уведомления", icon: <Bell className="w-5 h-5" />, path: "/notifications" },
    ];

    return (
        <div className="fixed bottom-0 left-0 flex justify-center w-full shadow-2xl border-t-1 bg-white">
            <div className="flex justify-center py-2 z-50 bg-white  max-w-md w-full"
                 style={{ paddingBottom: 'env(safe-area-inset-bottom)' }}>
                {navItems.map((item) => {
                    const isActive = pathname === item.path;
                    return (
                        <Button
                            key={item.label}
                            onClick={() => navigate(item.path)}
                            variant="ghost"
                            className={`flex-1 flex flex-col items-center justify-center gap-1 mb-1.5 mt-1.5 ${
                                isActive ? "text-blue-600" : "text-gray-500 hover:text-gray-900"
                            }`}
                        >
                            {item.icon}
                            <span className="text-[10px] font-medium">{item.label}</span>
                        </Button>
                    );
                })}
            </div>
        </div>
    );
};