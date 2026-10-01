import type { ReactNode } from 'react';
import { ArrowUpRight, LoaderCircle } from 'lucide-react';
import { Link } from 'react-router-dom';
import type { Block } from './types';

export function Notice({ error, message }: { error?: string; message?: string }) {
  return <>{error && <p className="notice error" role="alert">{error}</p>}{message && <p className="notice success" role="status">{message}</p>}</>;
}
export function Loading() { return <div className="loading" role="status"><LoaderCircle className="spin" size={22} /> Загружаем…</div>; }
export function Empty({ children }: { children: ReactNode }) { return <div className="empty">{children}</div>; }
export function Heading({ eyebrow, title, children }: { eyebrow: string; title: string; children?: ReactNode }) {
  return <div className="page-heading"><div><span className="eyebrow">{eyebrow}</span><h1>{title}</h1></div>{children}</div>;
}
export function Blocks({ blocks }: { blocks: Block[] }) {
  return <div className="block-grid">{blocks.filter(b => b.visible).map((block, i) => <article className="card content-block" key={i}><span className="eyebrow">От вашего клуба</span><h3>{block.title}</h3><p>{block.text}</p></article>)}</div>;
}
export function TextLink({ to, children }: { to: string; children: ReactNode }) { return <Link className="text-link" to={to}>{children}<ArrowUpRight size={17} /></Link>; }
export const date = (value: string) => new Date(value).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' });
export const money = (value: number) => new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(value);
