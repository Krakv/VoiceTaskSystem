import { useAuthStore } from '@/stores/authStore';
import { Route, Routes, Navigate } from 'react-router-dom';
import { privateRoutes, publicRoutes } from '@/router';
import { Layout } from './Layout';

const AppRouter = () => {
    const isAuth = useAuthStore((state) => state.isAuth);

    return (
        <Routes>
            {isAuth
                ? privateRoutes.map((route) => (
                    <Route element={<Layout />}>
                        <Route
                            key={route.path}
                            path={route.path}
                            element={<route.component />}
                        />
                    </Route>
                ))
                : publicRoutes.map((route) => (
                    <Route
                        key={route.path}
                        path={route.path}
                        element={<route.component />}
                    />
                ))}
            <Route
                path="*"
                element={isAuth ? <Navigate to="/tasks" replace /> : <Navigate to="/login" replace />}
            />
        </Routes>
    );
};

export default AppRouter;