import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowUpRight, Search, Users } from 'lucide-react';
import { useApi, useAction } from '../hooks';
import { api } from '../api';
import { date, Empty, Heading, Loading, Notice } from '../ui';
import type { Trainer } from '../types';

export const specializations: Record<string, string> = { Strength: 'Силовые', WeightLoss: 'Снижение веса', Crossfit: 'Кроссфит', Yoga: 'Йога', Rehabilitation: 'Реабилитация', Running: 'Бег' };
export const experiences: Record<string, string> = { LessThanOneYear: 'До 1 года', OneToThreeYears: '1–3 года', ThreeToSevenYears: '3–7 лет', SevenPlusYears: 'Более 7 лет' };
export type Slot = { id: string; startUtc: string; endUtc: string; note?: string; isBooked: boolean };

export function Trainers() {
  const { data, error, loading } = useApi<Trainer[]>('/api/web/trainers');
  const [search, setSearch] = useState(''); const [selected, setSelected] = useState<Trainer>();
  const filtered = data?.filter(t => `${t.name} ${t.about}`.toLocaleLowerCase().includes(search.toLocaleLowerCase()));
  return <><Heading eyebrow="ЛЮДИ, КОТОРЫЕ ПОМОГУТ" title="Тренеры клуба" /><div className="search-field"><Search size={18} /><input aria-label="Поиск тренера" placeholder="Имя или направление" value={search} onChange={e => setSearch(e.target.value)} /></div><Notice error={error} />{loading ? <Loading /> : !filtered?.length ? <Empty><Users size={32} /><h3>Пока нет подходящих тренеров</h3><p>Карточки появятся, когда клуб добавит тренеров.</p></Empty> : <div className="trainer-grid">{filtered.map((t, index) => <article className="card trainer-card" key={t.id}><div className={`trainer-cover cover-${index % 3}`}><span>{t.name.slice(0, 1)}</span><small>ТРЕНЕР КЛУБА</small></div><div className="trainer-info"><span className="eyebrow">{t.specializations.split(', ').map(s => specializations[s] ?? s).join(' · ')}</span><h2>{t.name}</h2><p className="muted">Опыт: {experiences[t.experience] ?? t.experience}</p><p>{t.about || 'Помогу сделать следующий шаг к твоей цели.'}</p><div className="trainer-price"><strong>{t.pricePerSession} FC<small> / занятие</small></strong><button aria-label={`Открыть карточку ${t.name}`} className="round-button" onClick={() => setSelected(t)}><ArrowUpRight size={20} /></button></div></div></article>)}</div>}{selected && <TrainerDialog trainer={selected} close={() => setSelected(undefined)} />}</>;
}

function TrainerDialog({ trainer, close }: { trainer: Trainer; close: () => void }) {
  const slots = useApi<Slot[]>(`/api/schedule/slots?trainerId=${encodeURIComponent(trainer.userId)}`);
  const [message, setMessage] = useState(''); const action = useAction(); const navigate = useNavigate();
  return <div className="dialog-backdrop"><section className="card dialog" role="dialog" aria-modal="true" aria-labelledby="trainer-name"><div className="section-heading"><h2 id="trainer-name">{trainer.name}</h2><button className="secondary" onClick={close}>Закрыть</button></div><p>{trainer.about}</p><h3>Заявка на занятия</h3><label>Расскажи о своей цели<textarea maxLength={1000} value={message} onChange={e => setMessage(e.target.value)} placeholder="Чего хочешь достичь?" /></label><button className="primary" disabled={action.busy} onClick={() => action.run(async () => { await api('/api/trainers/requests', 'POST', { trainerUserId: trainer.userId, goal: trainer.specializations, formats: trainer.formats, level: 'Beginner', message, budget: trainer.pricePerSession }); }, 'Заявка отправлена тренеру. Теперь можно открыть диалог.')}>Отправить заявку</button><button className="secondary" disabled={action.busy} onClick={() => action.run(async () => { const result = await api<{ id: string }>('/api/chat/conversations', 'POST', { otherUserId: trainer.userId }); navigate(`/chats?conversation=${result.id}`); })}>Открыть диалог</button>
    <h3>Свободное время</h3><Notice error={slots.error} />{slots.loading ? <Loading /> : !slots.data?.length ? <p className="muted">Свободных слотов пока нет. Оставь заявку тренеру.</p> : slots.data.map(slot => <div className="list-row" key={slot.id}><span>{date(slot.startUtc)}<small>{new Date(slot.startUtc).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })} · {slot.note}</small></span><button className="secondary" disabled={action.busy} onClick={() => action.run(async () => { await api('/api/schedule/bookings', 'POST', { slotId: slot.id, note: message }); slots.reload(); }, 'Запись создана. Следи за статусом в разделе «Мои записи».')}>Записаться</button></div>)}<Notice {...action} /></section></div>;
}
