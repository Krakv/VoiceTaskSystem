import { Outlet } from "react-router-dom";
import { BottomNav } from "@/components/BottomNav";
import Header from "@/components/Header.tsx";

export const Layout = () => {
    return (
        <div className="flex justify-center min-h-screen bg-white">
            <div className="flex flex-col w-full max-w-md min-h-screen bg-white relative">
                <Header/>

                <main className="flex-1 p-4 overflow-auto pb-32">
                    <Outlet />
                </main>

                <BottomNav />
            </div>
        </div>
    );
};