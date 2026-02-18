import { useAuthStore } from '@/stores/authStore';
import { Link, useLocation } from 'react-router-dom';
import {Button} from "@chakra-ui/react";

const Header = () => {
    const isAuth = useAuthStore((state) => state.isAuth);
    const logout = useAuthStore((state) => state.logout);
    const location = useLocation();

    return (
        <header className="bg-background border-b fixed top-0 left-0 w-full z-50">
            <div className="container mx-auto flex h-16 items-center justify-between px-4">

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
                                    location.pathname === '/register' ? 'solid' : 'ghost'
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