import { useAuthStore } from '@/stores/authStore';
import { Link, useLocation } from 'react-router-dom';
import {Button} from "@chakra-ui/react";

const Header = () => {
    const isAuth = useAuthStore((state) => state.isAuth);
    const logout = useAuthStore((state) => state.logout);
    const location = useLocation();

    return (
        <header className="bg-background border-b">
            <div className="container mx-auto flex h-16 items-center justify-between px-4">
                <div className="flex items-center space-x-6">
                    <nav className="hidden md:flex space-x-4">
                        {isAuth && (
                            <>
                                <Button
                                    variant={location.pathname === '/main' ? 'solid' : 'outline'}
                                >
                                    <Link to="/main">Список задач</Link>
                                </Button>
                                <Button
                                    variant={location.pathname === '/create' ? 'solid' : 'outline'}
                                >
                                    <Link to="/create">Создать задачу</Link>
                                </Button>
                            </>
                        )}
                    </nav>
                </div>

                <div className="flex items-center space-x-4">
                    {isAuth ? (
                        <>
                            <Button variant="outline" onClick={logout}>
                                Выйти
                            </Button>
                        </>
                    ) : (
                        <>
                            <Button
                                asChild
                                variant={
                                    location.pathname === '/login' ? 'solid' : 'outline'
                                }
                            >
                                <Link to="/login">Войти</Link>
                            </Button>
                            <Button
                                asChild
                                variant={
                                    location.pathname === '/register' ? 'solid' : 'outline'
                                }
                            >
                                <Link to="/register">Зарегистрироваться</Link>
                            </Button>
                        </>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Header;