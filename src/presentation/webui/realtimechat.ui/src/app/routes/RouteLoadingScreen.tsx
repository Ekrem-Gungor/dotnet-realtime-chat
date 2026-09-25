export function RouteLoadingScreen() {
  return (
    <main className="app-shell">
      <section className="app-placeholder" aria-live="polite" aria-busy="true">
        <span className="app-brand">Siglora</span>
        <h1>Oturum doğrulanıyor</h1>
        <p>Lütfen kısa bir süre bekleyin.</p>
      </section>
    </main>
  );
}
