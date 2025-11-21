import { createContext, useContext } from "react";
import type { ReactNode } from "react";
import { ApiClient } from "./ApiClient.Generated";

const ApiClientContext = createContext<ApiClient | null>(null);

export function ApiClientProvider({
  children,
  apiClient,
}: {
  children: ReactNode;
  apiClient: ApiClient;
}) {
  return (
    <ApiClientContext.Provider value={apiClient}>
      {children}
    </ApiClientContext.Provider>
  );
}

export function useApiClient(): ApiClient {
  const context = useContext(ApiClientContext);
  if (!context) {
    throw new Error("useApiClient must be used within ApiClientProvider");
  }
  return context;
}

