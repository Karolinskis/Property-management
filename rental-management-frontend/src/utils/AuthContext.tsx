import React, { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

interface DecodedToken {
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string[];
}

interface AuthContextType {
  roles: string[];
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [roles, setRoles] = useState<string[]>([]);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      const decodedToken = jwtDecode<DecodedToken>(token);

      console.log("Decoded token", decodedToken);

      setRoles(
        decodedToken[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ] || []
      );
      setIsAuthenticated(true);
    }
  }, []);

  const login = (token: string) => {
    localStorage.setItem("accessToken", token);
    const decodedToken = jwtDecode<DecodedToken>(token);
    setRoles(
      decodedToken[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ] || []
    );
    setIsAuthenticated(true);
  };

  const logout = () => {
    localStorage.removeItem("accessToken");
    setRoles([]);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ roles, isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
