import { Navigate, Outlet } from "react-router";
import { useAuth } from "../../features/auth/context/useAuth";
import { RouteLoadingScreen } from "./RouteLoadingScreen";

export function ProtectedRoute() {
  const { status } = useAuth();

  if (status === "checking") {
    return <RouteLoadingScreen />;
  }

  if (status === "anonymous") {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
