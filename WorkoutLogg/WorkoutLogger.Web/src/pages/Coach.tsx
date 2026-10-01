import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Sparkles, Send } from 'lucide-react';
import { useApp } from '../App';
import { api } from '../api';
import { useAction, useApi } from '../hooks';
import { date, Empty, Heading, Loading, Notice } from '../ui';
import { workoutNames, type Workout } from '../types';

export function Coach() {
  const { user } = useApp(); const action = useAction(); const [text, setText] = useState('');
  const [messages, setMessages] = useState<{ role: string; content: string }[]>([]);
  const history = useApi<Workout[]>('/Workouts?pageSize=20');
  const progress = useApi<{ totalSessions: number; currentStreak: number; records: { name: string; weightKg: number }[] }>('/api/web/progress');
  const context = { totalSessions: progress.data?.totalSessions ?? 0, currentStreak: progress.data?.currentStreak ?? 0, personalRecords: progress.data?.records.map(r => r.name + ': ' + r.weightKg + ' кг').join(', '), recentSummary: history.data?.map(w => `${date(w.startDate)}: ${workoutNames[w.workoutType]}, ${w.exerciseCount} упражнений`).join('\n') };
  async function ask(kind: 'chat' | 'plan' | 'forecast') {
    if (!progress.data || !history.data) throw new Error('История тренировок ещё не загружена. Попробуйте позже.');
    const next = [...messages, { role: 'user', content: text }];
    const body = kind === 'chat' ? { messages: next.slice(-40), context, language: 'ru' } : { context, language: 'ru', goal: text, daysPerWeek: 3 };
    const response = await api<{ content: string; success: boolean; error?: string }>(`/api/ai/${kind}`, 'POST', body);
    if (!response.success) throw new Error(response.error ?? 'AI-коуч временно недоступен');
    setMessages([...next, { role: 'assistant', content: response.content }]); setText('');
  }
  return <><Heading eyebrow="ПОДДЕРЖКА НА ПУТИ К ЦЕЛИ" title="AI-коуч" />{!user.isPremium ? <Empty><Sparkles size={34} /><h3>Твой помощник по тренировкам</h3><p>Чат, план занятий и прогноз прогресса доступны с Premium.</p><Link className="button primary" to="/premium">Моя подписка</Link></Empty> : <section className="card"><div className="messages">{!messages.length ? <Empty><Sparkles size={28} /><h3>С чего начнём?</h3><p>Расскажи о своей цели или задай вопрос о тренировке.</p></Empty> : messages.map((m, i) => <article key={i} className={`message ${m.role === 'user' ? 'mine' : ''}`}><p>{m.content}</p></article>)}</div><form onSubmit={e => { e.preventDefault(); void action.run(() => ask('chat')); }}><label>Сообщение<textarea required maxLength={4000} value={text} onChange={e => setText(e.target.value)} /></label><div className="form-actions"><button className="primary" disabled={action.busy || !text.trim()}><Send size={17} />Спросить</button><button className="secondary" type="button" disabled={action.busy} onClick={() => action.run(() => ask('plan'))}>Составить план</button><button className="secondary" type="button" disabled={action.busy} onClick={() => action.run(() => ask('forecast'))}>Прогноз прогресса</button></div><Notice {...action} /></form></section>}</>;
}

type Subscription = { isActive: boolean; plan?: string; status?: string; expiresAt?: string };
export function Premium() {
  const source = useApi<Subscription>('/api/subscriptions/status'); const action = useAction(); const { reloadUser } = useApp();
  return <><Heading eyebrow="БОЛЬШЕ ВОЗМОЖНОСТЕЙ" title="Моя подписка" /><Notice error={source.error} />{source.loading ? <Loading /> : <section className="card"><Sparkles size={28} /><h2 style={{ marginTop:20 }}>{source.data?.isActive ? 'Premium активен' : 'Обычный аккаунт'}</h2>{source.data?.expiresAt && <p>Действует до {date(source.data.expiresAt)}</p>}<p className="muted">Premium открывает AI-коуча, генерацию плана и прогноз прогресса.</p><div className="form-actions"><button className="secondary" disabled={action.busy} onClick={() => action.run(async () => { await api('/api/subscriptions/restore', 'POST'); source.reload(); await reloadUser(); }, 'Статус подписки обновлён')}>Восстановить покупку</button></div><Notice {...action} /><p className="muted" style={{ marginTop:20 }}>Подключение и условия подписки уточняйте у администратора клуба.</p></section>}</>;
}
