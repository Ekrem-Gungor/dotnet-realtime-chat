import { Navigate, Outlet } from "react-router";
import { useAuth } from "../../features/auth/context/useAuth";
import { RouteLoadingScreen } from "./RouteLoadingScreen";

export function PublicOnlyRoute() {
  const { status } = useAuth();

  if (status === "checking") {
    return <RouteLoadingScreen />;
  }

  if (status === "authenticated") {
    return <Navigate to="/chat" replace />;
  }

  return <Outlet />;
}
