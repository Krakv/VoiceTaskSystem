import Header from '@/components/Header';
import { Outlet } from 'react-router-dom';

export const Layout = () => {
    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="flex-1 py-6">
                <Outlet />
            </main>
        </div>
    );
};