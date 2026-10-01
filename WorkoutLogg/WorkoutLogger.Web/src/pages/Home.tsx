import { useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowUpRight, Dumbbell, Flame, LogOut, Plus, Target } from 'lucide-react';
import { useApp } from '../App';
import { api } from '../api';
import { useAction, useApi } from '../hooks';
import { Blocks, date, Empty, Heading, Loading, Notice, TextLink } from '../ui';
import { workoutNames, type Workout } from '../types';

export function Dashboard() {
  const { user, site } = useApp();
  const { data, loading, error } = useApi<Workout[]>('/Workouts?pageSize=200');
  const workouts = data ?? [];
  const progress = useApi<{ monthSessions: number; monthMinutes: number }>('/api/web/progress');
  const month = workouts.filter(w => new Date(w.startDate).getMonth() === new Date().getMonth() && new Date(w.startDate).getFullYear() === new Date().getFullYear());
  const minutes = month.reduce((sum, w) => sum + Math.max(0, Math.round((+new Date(w.endDate) - +new Date(w.startDate)) / 60000)), 0);
  return <><Heading eyebrow="ТВОЙ ЛИЧНЫЙ РИТМ" title={`Привет, ${user.fullName?.split(' ')[0] || 'спортсмен'} 👋`}><span className="date-chip">{date(new Date().toISOString())}</span></Heading>
    <section className="hero"><div><span className="eyebrow">МАЛЕНЬКИЕ ШАГИ. БОЛЬШИЕ ИЗМЕНЕНИЯ.</span><h2>{site.brand.tagline}</h2><p>Всё, что нужно для движения вперёд, — в одном месте.</p><Link className="button primary" to="/workouts/new">Начать тренировку<ArrowUpRight size={20} /></Link></div><div className="hero-art" aria-hidden="true"><div className="hero-disc"><Dumbbell strokeWidth={1.1} size={110} /></div><span>KEEP<br />MOVING.</span></div></section>
    <Notice error={error} />{loading ? <Loading /> : <div className="stats-grid">{[{ title: 'Тренировок в этом месяце', value: progress.data?.monthSessions ?? month.length, unit: 'занятий', icon: Dumbbell }, { title: 'Время в движении', value: progress.data?.monthMinutes ?? minutes, unit: 'минут за месяц', icon: Flame }, { title: 'Твой вес', value: user.bodyStats?.kg || '—', unit: 'кг · из профиля', icon: Target }].map(({ title, value, unit, icon: Icon }) => <article className="card stat" key={title}><div><span>{title}</span><Icon size={19} /></div><strong>{value}</strong><small>{unit}</small></article>)}</div>}
    <div className="section-heading"><h2>Последние тренировки</h2><TextLink to="/workouts">Вся история</TextLink></div><div className="card list-card">{workouts.length === 0 && !loading ? <Empty><Dumbbell size={32} /><h3>Первая тренировка впереди</h3><p>Добавь упражнения и подходы — сохраним твой результат.</p><Link to="/workouts/new" className="button secondary"><Plus size={18} />Добавить тренировку</Link></Empty> : workouts.slice(0, 4).map(w => <Link className="list-row" key={w.id} to={`/workouts/${w.id}`}><span className="row-icon"><Dumbbell size={22} /></span><span><strong>{workoutNames[w.workoutType] ?? w.workoutType}</strong><small>{date(w.startDate)} · Упражнений: {w.exerciseCount}</small></span><ArrowUpRight size={20} /></Link>)}</div>
    <Blocks blocks={site.brand.homeBlocks} />
  </>;
}

export function Profile({ onLogout }: { onLogout: () => Promise<void> }) {
  const { user, reloadUser, site, theme, changeTheme } = useApp();
  const [name, setName] = useState(user.fullName); const [kg, setKg] = useState(user.bodyStats?.kg ?? 0); const [cm, setCm] = useState(user.bodyStats?.cm ?? 0);
  const [goals, setGoals] = useState(user.goals?.map(g => g.goal) ?? []);
  const [frequency, setFrequency] = useState(user.workOutCount ?? 'Three');
  const goalNames: Record<string,string> = { LoseFat:'Снизить вес', BuildMuscle:'Набрать мышцы', ImporveEndurance:'Развить выносливость', IncreaseStrength:'Стать сильнее', Flexibility:'Улучшить гибкость', StayActive:'Поддерживать активность' };
  const action = useAction();
  return <><Heading eyebrow="ТВОЙ АККАУНТ" title="Профиль"><button className="secondary" disabled={action.busy} onClick={() => action.run(onLogout)}><LogOut size={18} />Выйти</button></Heading>
    <div className="profile-layout"><section className="card"><h2>Личные данные</h2><p className="muted">{user.email || user.phoneNumber}</p><form onSubmit={e => { e.preventDefault(); void action.run(async () => { await api('/Auth/UpdateAccount', 'PUT', { fullName: name, goals: goals.map(goal => ({ goal })), workOutCount: frequency, userRegistrationStep: 'Finished', bodyStats: { kg, cm, fat: user.bodyStats?.fat ?? 0 } }); await reloadUser(); }, 'Профиль сохранён'); }}>
      <label>Имя<input required maxLength={80} value={name} onChange={e => setName(e.target.value)} /></label><div className="form-grid"><label>Вес, кг<input type="number" min={1} max={500} step="0.1" required value={kg} onChange={e => setKg(+e.target.value)} /></label><label>Рост, см<input type="number" min={50} max={260} required value={cm} onChange={e => setCm(+e.target.value)} /></label></div><label>Тренировок в неделю<select value={frequency} onChange={e => setFrequency(e.target.value)}>{["One","Two","Three","Four","Five","Six"].map((value,i) => <option key={value} value={value}>{i + 1}</option>)}</select></label><fieldset className="goals"><legend>Твои цели</legend>{Object.entries(goalNames).map(([value,title]) => <label key={value} className="checkbox"><input type="checkbox" checked={goals.includes(value)} onChange={e => setGoals(e.target.checked ? [...goals,value] : goals.filter(g => g !== value))} />{title}</label>)}</fieldset><button className="primary" disabled={action.busy}>Сохранить изменения</button><Notice {...action} /></form></section>
      <section className="card"><h2>Внешний вид</h2><p className="muted">Выбери тему, в которой тебе комфортно.</p><div className="theme-options">{[{ value: 'light', title: 'Светлая' }, { value: 'dark', title: 'Тёмная' }, { value: 'system', title: 'Как на устройстве' }].map(t => <button className={theme === t.value ? 'selected' : ''} aria-pressed={theme === t.value} key={t.value} onClick={() => changeTheme(t.value)}><span className={`theme-swatch ${t.value}`} />{t.title}</button>)}</div><p className="muted">Настройка сохраняется в этом браузере для твоего клуба.</p><details><summary>Идентификатор аккаунта</summary><code className="user-id">{user.id}</code><p className="muted">Администратор платформы использует его для назначения владельца клуба.</p></details></section></div><div className="form-actions"><Link className="button secondary" to="/premium">Моя подписка</Link><Link className="button secondary" to="/standards">Силовые показатели</Link></div><Blocks blocks={site.brand.profileBlocks} />
  </>;
}
