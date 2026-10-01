import { useCallback, useEffect, useState } from 'react';
import { api } from './api';

export function useApi<T>(path: string | null) {
  const [data, setData] = useState<T>();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [version, setVersion] = useState(0);
  const reload = useCallback(() => setVersion(v => v + 1), []);
  useEffect(() => {
    let active = true;
    setData(undefined); setError(''); setLoading(!!path);
    if (path) api<T>(path).then(value => { if (active) setData(value); })
      .catch(e => { if (active) setError(e.message); }).finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, [path, version]);
  return { data, error, loading, reload };
}

export function useAction() {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  async function run(action: () => Promise<void>, success = '') {
    if (busy) return;
    setBusy(true); setError(''); setMessage('');
    try { await action(); setMessage(success); } catch (e) { setError(e instanceof Error ? e.message : 'Ошибка запроса'); }
    finally { setBusy(false); }
  }
  return { busy, error, message, run };
}
