import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router";
import { describe, expect, it, vi } from "vitest";
import {
  AuthContext,
  type AuthContextValue,
} from "../../../features/auth/context/authContext";
import { ProtectedRoute } from "../ProtectedRoute";

function renderProtectedRoute(status: AuthContextValue["status"]) {
  const contextValue: AuthContextValue = {
    status,
    user:
      status === "authenticated"
        ? {
            userId: 4,
            userName: "realtime.demo",
            email: "realtime.demo@example.invalid",
            roles: ["Member"],
          }
        : null,
    sessionError: null,
    login: vi.fn(),
    logout: vi.fn(),
  };

  render(
    <AuthContext.Provider value={contextValue}>
      <MemoryRouter initialEntries={["/chat"]}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/chat" element={<h1>Sohbet alanı</h1>} />
          </Route>

          <Route path="/login" element={<h1>Giriş ekranı</h1>} />
        </Routes>
      </MemoryRouter>
    </AuthContext.Provider>,
  );
}

describe("ProtectedRoute", () => {
  it("oturum açmış kullanıcıya korunan sayfayı gösterir", () => {
    renderProtectedRoute("authenticated");

    expect(
      screen.getByRole("heading", { name: "Sohbet alanı" }),
    ).toBeInTheDocument();
  });

  it("anonim kullanıcıyı giriş ekranına yönlendirir", () => {
    renderProtectedRoute("anonymous");

    expect(
      screen.getByRole("heading", { name: "Giriş ekranı" }),
    ).toBeInTheDocument();
  });

  it("oturum kontrol edilirken yükleme ekranını gösterir", () => {
    renderProtectedRoute("checking");

    expect(screen.getByText(/oturum doğrulanıyor/i)).toBeInTheDocument();
  });
});
