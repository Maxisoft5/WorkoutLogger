import { Memberships } from './pages/Memberships';
import { createContext, useContext, useEffect, useState } from 'react';
import { Link, NavLink, Navigate, Route, Routes, useLocation } from 'react-router-dom';
import { Activity, ArrowUpRight, CalendarDays, Dumbbell, Home, LogOut, MessageSquare, Settings2, Shield, Sparkles, Users, Wallet } from 'lucide-react';
import { api, restoreSession, setToken } from './api';
import type { Brand, Site, User } from './types';
import { useAction } from './hooks';
import { Loading, Notice } from './ui';
import { Dashboard, Profile } from './pages/Home';
import { Workouts, WorkoutEditor } from './pages/Workouts';
import { Trainers } from './pages/Trainers';
import { Admin } from './pages/Admin';
import { Bookings, Chats, WalletPage } from './pages/Community';
import { Coach, Premium } from './pages/Coach';
import { Standards } from './pages/Standards';
import { TrainerCabinet } from './pages/TrainerCabinet';

type AppState = { site: Site; user: User; isAdmin: boolean; isTrainer: boolean; reloadUser: () => Promise<void>; reloadSite: () => Promise<void>; theme: string; changeTheme: (value: string) => void };
const Context = createContext<AppState | null>(null);
export function useApp() { return useContext(Context)!; }

export default function App() {
  const [site, setSite] = useState<Site>();
  const [user, setUser] = useState<User>();
  const [isAdmin, setAdmin] = useState(false);
  const [isTrainer, setTrainer] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [theme, setTheme] = useState('system');
  const location = useLocation();
  async function reloadSite() { setSite(await api<Site>('/api/site')); }
  async function reloadUser() {
    const [me, access] = await Promise.all([api<User>('/Auth/CurrentUser'), api<{ isAdmin: boolean; isTrainer: boolean }>('/api/site/access')]);
    setUser(me); setAdmin(access.isAdmin); setTrainer(access.isTrainer);
  }
  useEffect(() => {
    (async () => {
      try { await reloadSite(); if (await restoreSession()) await reloadUser(); }
      catch (e) { setError((e as Error).message); }
      finally { setLoading(false); }
    })();
    const expired = () => { setUser(undefined); setToken(null); };
    window.addEventListener('session-expired', expired);
    return () => window.removeEventListener('session-expired', expired);
  }, []);
  useEffect(() => {
    if (!site) return;
    setTheme(localStorage.getItem(`appearance:${site.tenantId}`) ?? site.brand.defaultTheme);
    document.title = site.brand.name;
    document.documentElement.style.setProperty('--accent', site.brand.accent);
    // Select an accessible foreground for any administrator-selected accent.
    const channels = site.brand.accent.slice(1).match(/.{2}/g)!.map(c => parseInt(c, 16) / 255).map(c => c <= .04045 ? c / 12.92 : ((c + .055) / 1.055) ** 2.4);
    const luminance = channels[0] * .2126 + channels[1] * .7152 + channels[2] * .0722;
    document.documentElement.style.setProperty('--on-accent', luminance > .179 ? '#111111' : '#ffffff');
    document.documentElement.dataset.shape = site.brand.shape;
  }, [site]);
  useEffect(() => {
    const media = matchMedia('(prefers-color-scheme: dark)');
    const apply = () => { document.documentElement.dataset.theme = theme === 'system' ? (media.matches ? 'dark' : 'light') : theme; };
    apply(); media.addEventListener('change', apply);
    return () => media.removeEventListener('change', apply);
  }, [theme]);
  useEffect(() => { window.scrollTo(0, 0); }, [location.pathname]);
  const changeTheme = (value: string) => { setTheme(value); localStorage.setItem(`appearance:${site!.tenantId}`, value); };
  if (loading) return <main className="boot"><Loading /></main>;
  if (!site) return <main className="boot"><Notice error={error || 'Не удалось загрузить сайт зала.'} /><button onClick={() => window.location.reload()}>Попробовать снова</button></main>;
  if (!user) return <Login brand={site.brand} onLogin={reloadUser} initialError={error} />;
  const navigation = [
    { to: '/', label: 'Главная', icon: Home }, { to: '/workouts', label: 'Тренировки', icon: Dumbbell },
    { to: '/trainers', label: 'Тренеры', icon: Users }, { to: '/bookings', label: 'Мои записи', icon: CalendarDays },
    { to: '/chats', label: 'Сообщения', icon: MessageSquare }, { to: '/wallet', label: 'Кошелёк', icon: Wallet },
    { to: '/coach', label: 'AI-коуч', icon: Sparkles }, { to: '/memberships', label: 'Абонементы', icon: CalendarDays },
    ...(isTrainer ? [{ to: '/trainer', label: 'Кабинет тренера', icon: CalendarDays }] : []),
  ];
  return <Context.Provider value={{ site, user, isAdmin, isTrainer, reloadUser, reloadSite, theme, changeTheme }}>
    <div className="app-shell"><aside className="sidebar">
      <Link to="/" className="brand"><span className="brand-mark"><Activity size={24} /></span><span>{site.brand.name}<small>ТЕРРИТОРИЯ ДВИЖЕНИЯ</small></span></Link>
      <span className="nav-caption">ТВОЙ КЛУБ</span>
      <nav aria-label="Основная навигация">{navigation.map(({ to, label, icon: Icon }) => <NavLink key={to} to={to} end={to === '/'}><Icon size={20} />{label}</NavLink>)}</nav>
      <div className="sidebar-bottom">{isAdmin && <NavLink className="admin-link" to="/admin"><Shield size={19} />Управление клубом</NavLink>}
        <NavLink className="profile-link" to="/profile"><span className="avatar">{user.fullName?.slice(0, 1).toUpperCase() || 'Я'}</span><span>{user.fullName || 'Мой профиль'}<small>Личный кабинет</small></span><Settings2 size={17} /></NavLink>
      </div>
    </aside><div className="workspace"><header className="topbar"><span className="club-status"><i />Твой прогресс начинается здесь</span><Link to="/profile">{user.fullName?.split(' ')[0] || 'Профиль'}<span className="avatar small">{user.fullName?.slice(0, 1) || 'Я'}</span></Link></header>
      <main className="main"><Routes>
        <Route path="/" element={<Dashboard />} /><Route path="/workouts" element={<Workouts />} />
        <Route path="/workouts/new" element={<WorkoutEditor />} /><Route path="/workouts/:id" element={<WorkoutEditor />} />
        <Route path="/profile" element={<Profile onLogout={async () => { await api('/api/session/logout', 'POST'); setToken(null); setUser(undefined); }} />} />
        <Route path="/trainers" element={<Trainers />} /><Route path="/bookings" element={<Bookings />} /><Route path="/chats" element={<Chats />} /><Route path="/wallet" element={<WalletPage />} />
        <Route path="/memberships" element={<Memberships />} /><Route path="/coach" element={<Coach />} /><Route path="/premium" element={<Premium />} /><Route path="/standards" element={<Standards />} /><Route path="/trainer" element={isTrainer ? <TrainerCabinet /> : <Navigate to="/" replace />} />
        <Route path="/admin" element={isAdmin ? <Admin /> : <Navigate to="/" replace />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes></main><footer>{site.brand.name}<span>Сильнее с каждой тренировкой.</span></footer>
    </div></div>
  </Context.Provider>;
}

function Login({ brand, onLogin, initialError }: { brand: Brand; onLogin: () => Promise<void>; initialError: string }) {
  const [mode, setMode] = useState<'login' | 'register' | 'reset'>('login');
  const [contactKind, setContactKind] = useState<'email' | 'phone'>('email');
  const [email, setEmail] = useState(''); const [password, setPassword] = useState(''); const [name, setName] = useState(''); const [code, setCode] = useState(''); const [codeSent, setCodeSent] = useState(false);
  const action = useAction();
  return <main className="auth-shell"><section className="auth-story"><Link to="/" className="brand"><Activity />{brand.name}</Link><div><span className="eyebrow">КАЖДЫЙ ДЕНЬ — НОВАЯ ВОЗМОЖНОСТЬ</span><h1>Становись<br />сильнее.<br /><em>По-своему.</em></h1><p>{brand.tagline}</p></div><span className="auth-foot"><Sparkles size={18} />Тренировки. Люди. Твой прогресс.</span><div className="orbit" aria-hidden="true" /></section>
      <section className="auth-form"><div className="auth-form-inner"><span className="eyebrow">{brand.name}</span><h2>{mode === 'login' ? 'С возвращением' : mode === 'register' ? 'Начнём твою историю' : 'Восстановить доступ'}</h2><p className="muted">{mode === 'login' ? 'Войди в свой аккаунт клуба.' : mode === 'register' ? 'Создай аккаунт и запиши первую тренировку.' : 'Пришлём код на твою почту.'}</p>
        {mode !== 'reset' && <div className="tabs" aria-label="Способ входа">{[{id:'email',label:'По почте'},{id:'phone',label:'По телефону'}].map(t=><button type="button" key={t.id} className={contactKind===t.id?'active':''} aria-pressed={contactKind===t.id} onClick={()=>{setContactKind(t.id as 'email'|'phone');setEmail('');}}>{t.label}</button>)}</div>}
        <form onSubmit={e => { e.preventDefault(); void action.run(async () => {
          if (mode === 'reset') {
            if (!codeSent) { await api('/Auth/ForgotPassword', 'POST', { email }); setCodeSent(true); }
            else { await api('/Auth/ResetPassword', 'POST', { email, code, newPassword: password }); setMode('login'); setCodeSent(false); }
            return;
          }
          const result = await api<{ token: string }>(`/api/session/${mode}`, 'POST', { ...(contactKind === 'phone' ? { phoneNumber: email } : { email }), password, fullName: name });
          setToken(result.token); await onLogin();
        }, mode === 'reset' ? 'Запрос выполнен. Проверь почту или войди с новым паролем.' : ''); }}>
          {mode === 'register' && <label>Имя<input autoComplete="name" required maxLength={80} value={name} onChange={e => setName(e.target.value)} /></label>}
                    <label>{mode !== 'reset' && contactKind === 'phone' ? 'Номер телефона' : 'Email'}<input type={mode !== 'reset' && contactKind === 'phone' ? 'tel' : 'email'} autoComplete={mode !== 'reset' && contactKind === 'phone' ? 'tel' : 'email'} maxLength={contactKind === 'phone' && mode !== 'reset' ? 40 : 256} placeholder={contactKind === 'phone' && mode !== 'reset' ? '+7 999 123-45-67' : 'you@example.com'} required value={email} onChange={e => setEmail(e.target.value)} /></label>
          {contactKind === 'phone' && mode !== 'reset' && <p className="muted">Вход по номеру и паролю. Подтверждение номера и восстановление через SMS пока недоступны.</p>}
          {mode === 'reset' && codeSent && <label>Код из письма<input inputMode="numeric" required value={code} onChange={e => setCode(e.target.value)} /></label>}
          {(mode !== 'reset' || codeSent) && <label>{mode === 'reset' ? 'Новый пароль' : 'Пароль'}<input type="password" autoComplete={mode === 'login' ? 'current-password' : 'new-password'} required minLength={8} maxLength={128} value={password} onChange={e => setPassword(e.target.value)} /></label>}
          <Notice error={action.error || initialError} message={action.message} /><button className="primary full" disabled={action.busy}>{action.busy ? 'Подождите…' : mode === 'login' ? 'Войти' : mode === 'register' ? 'Создать аккаунт' : codeSent ? 'Изменить пароль' : 'Получить код'}<ArrowUpRight size={18} /></button>
        </form><div className="auth-actions"><button className="link-button" onClick={() => setMode(mode === 'register' ? 'login' : 'register')}>{mode === 'register' ? 'Уже есть аккаунт? Войти' : 'Создать аккаунт'}</button><button className="link-button" onClick={() => { if (contactKind === 'phone') { setEmail(''); setContactKind('email'); } setMode(mode === 'reset' ? 'login' : 'reset'); }}>{mode === 'reset' ? 'Назад ко входу' : contactKind === 'phone' ? 'Восстановление по email' : 'Забыли пароль?'}</button></div>
      </div></section></main>;
}
