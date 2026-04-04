import { Outlet } from "react-router-dom";
import { BottomNav } from "@/components/BottomNav";
import Header from "@/components/Header.tsx";

export const Layout = () => {
    return (
        <div className="flex justify-center min-h-screen bg-gray-100">
            <div className="flex flex-col w-full max-w-md min-h-screen bg-gray-50 relative">
                <Header/>

                <main className="flex-1 p-4 overflow-auto">
                    <Outlet />
                </main>

                <BottomNav />
            </div>
        </div>
    );
};