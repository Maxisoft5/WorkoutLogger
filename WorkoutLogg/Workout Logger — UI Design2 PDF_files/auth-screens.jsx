// auth-screens.jsx
// Auth + Onboarding flow screens for Workout Logger
// Exports: AuthFlow (multi-step: Login/Register → Profile → Body Stats → Goals)

// ── Shared Input ─────────────────────────────────────────────
function AuthInput({ v, label, type = 'text', placeholder, value }) {
  return (
    <div style={{ marginBottom: 14 }}>
      <div style={{ color: v.textSub, fontSize: 11, fontWeight: 600, marginBottom: 6, letterSpacing: 0.5 }}>{label}</div>
      <div style={{
        background: v.inputBg, border: `1.5px solid ${v.cardBorder}`,
        borderRadius: 14, padding: '13px 16px', display: 'flex', alignItems: 'center', gap: 10,
      }}>
        <span style={{ color: v.text, fontSize: 14, fontWeight: 500, flex: 1, opacity: value ? 1 : 0.35 }}>
          {value || placeholder}
        </span>
      </div>
    </div>
  );
}

function AuthBtn({ v, children, secondary, onClick }) {
  return (
    <button onClick={onClick} style={{
      width: '100%', padding: '15px', borderRadius: 16,
      background: secondary ? 'transparent' : v.accentGrad,
      border: secondary ? `1.5px solid ${v.cardBorder}` : 'none',
      color: secondary ? v.textSub : '#fff',
      fontSize: 15, fontWeight: 700, cursor: 'pointer',
      fontFamily: 'inherit', letterSpacing: -0.2,
      boxShadow: secondary ? undefined : `0 6px 20px ${v.accentGlow}`,
      marginBottom: 10,
    }}>{children}</button>
  );
}

function SocialBtn({ v, icon, label }) {
  return (
    <div style={{
      flex: 1, background: v.card, border: `1.5px solid ${v.cardBorder}`,
      borderRadius: 14, padding: '12px', display: 'flex', alignItems: 'center',
      justifyContent: 'center', gap: 8, cursor: 'pointer',
      backdropFilter: v.glass ? 'blur(20px)' : undefined,
    }}>
      <span style={{ fontSize: 18 }}>{icon}</span>
      <span style={{ color: v.text, fontSize: 13, fontWeight: 600 }}>{label}</span>
    </div>
  );
}

// ── Step indicator ────────────────────────────────────────────
function StepDots({ v, total, current }) {
  return (
    <div style={{ display: 'flex', gap: 6, justifyContent: 'center', marginBottom: 28 }}>
      {Array.from({ length: total }).map((_, i) => (
        <div key={i} style={{
          height: 4, borderRadius: 99,
          width: i === current ? 24 : 8,
          background: i === current ? v.accent : `${v.accent}33`,
          transition: 'all 0.3s',
        }} />
      ))}
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// SCREEN 1 — LOGIN
// ══════════════════════════════════════════════════════════════
function LoginScreen({ v, onSwitch }) {
  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg, display: 'flex', flexDirection: 'column' }}>
      {/* Hero gradient top */}
      <div style={{
        padding: '48px 28px 36px', textAlign: 'center', position: 'relative',
        background: v.glass
          ? 'linear-gradient(180deg, rgba(124,58,237,0.18) 0%, transparent 100%)'
          : undefined,
      }}>
        <div style={{
          width: 64, height: 64, borderRadius: 20, background: v.accentGrad,
          margin: '0 auto 18px', display: 'flex', alignItems: 'center',
          justifyContent: 'center', fontSize: 28,
          boxShadow: `0 8px 28px ${v.accentGlow}`,
        }}>💪</div>
        <div style={{ color: v.text, fontSize: 26, fontWeight: 800, letterSpacing: -0.6 }}>Welcome back</div>
        <div style={{ color: v.textSub, fontSize: 14, marginTop: 6 }}>Sign in to continue your journey</div>
      </div>

      <div style={{ flex: 1, padding: '0 24px 32px' }}>
        <AuthInput v={v} label="EMAIL" placeholder="alex@example.com" value="alex@gmail.com" />
        <AuthInput v={v} label="PASSWORD" type="password" placeholder="••••••••" value="••••••••" />

        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 24, marginTop: -6 }}>
          <span style={{ color: v.accent, fontSize: 13, fontWeight: 600 }}>Forgot password?</span>
        </div>

        <AuthBtn v={v}>Sign In</AuthBtn>

        {/* Divider */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 12, margin: '18px 0' }}>
          <div style={{ flex: 1, height: 1, background: v.cardBorder }} />
          <span style={{ color: v.textMuted, fontSize: 12, fontWeight: 500 }}>or continue with</span>
          <div style={{ flex: 1, height: 1, background: v.cardBorder }} />
        </div>

        <div style={{ display: 'flex', gap: 10, marginBottom: 28 }}>
          <SocialBtn v={v} icon="🍎" label="Apple" />
          <SocialBtn v={v} icon="G" label="Google" />
        </div>

        <div style={{ textAlign: 'center' }}>
          <span style={{ color: v.textSub, fontSize: 13 }}>Don't have an account? </span>
          <span onClick={onSwitch} style={{ color: v.accent, fontSize: 13, fontWeight: 700, cursor: 'pointer' }}>Sign Up</span>
        </div>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// SCREEN 2 — REGISTER
// ══════════════════════════════════════════════════════════════
function RegisterScreen({ v, onSwitch, onNext }) {
  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg, display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '48px 28px 28px', textAlign: 'center' }}>
        <div style={{
          width: 64, height: 64, borderRadius: 20, background: v.accentGrad,
          margin: '0 auto 18px', display: 'flex', alignItems: 'center',
          justifyContent: 'center', fontSize: 28, boxShadow: `0 8px 28px ${v.accentGlow}`,
        }}>🚀</div>
        <div style={{ color: v.text, fontSize: 26, fontWeight: 800, letterSpacing: -0.6 }}>Create account</div>
        <div style={{ color: v.textSub, fontSize: 14, marginTop: 6 }}>Start your fitness journey today</div>
      </div>

      <div style={{ flex: 1, padding: '0 24px 32px' }}>
        <AuthInput v={v} label="FULL NAME" placeholder="Your name" value="Alex Petrov" />
        <AuthInput v={v} label="EMAIL" placeholder="you@example.com" value="alex@gmail.com" />
        <AuthInput v={v} label="PASSWORD" type="password" placeholder="Min. 8 characters" value="••••••••" />
        <AuthInput v={v} label="CONFIRM PASSWORD" type="password" placeholder="Repeat password" value="••••••••" />

        <div style={{ marginBottom: 20 }}>
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10 }}>
            <div style={{ width: 20, height: 20, borderRadius: 6, background: v.accentGrad, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0, marginTop: 1 }}>
              <span style={{ color: '#fff', fontSize: 12 }}>✓</span>
            </div>
            <span style={{ color: v.textSub, fontSize: 12, lineHeight: 1.5 }}>
              I agree to the <span style={{ color: v.accent }}>Terms of Service</span> and <span style={{ color: v.accent }}>Privacy Policy</span>
            </span>
          </div>
        </div>

        <AuthBtn v={v} onClick={onNext}>Create Account</AuthBtn>

        <div style={{ display: 'flex', alignItems: 'center', gap: 12, margin: '14px 0' }}>
          <div style={{ flex: 1, height: 1, background: v.cardBorder }} />
          <span style={{ color: v.textMuted, fontSize: 12 }}>or</span>
          <div style={{ flex: 1, height: 1, background: v.cardBorder }} />
        </div>

        <div style={{ display: 'flex', gap: 10, marginBottom: 20 }}>
          <SocialBtn v={v} icon="🍎" label="Apple" />
          <SocialBtn v={v} icon="G" label="Google" />
        </div>

        <div style={{ textAlign: 'center' }}>
          <span style={{ color: v.textSub, fontSize: 13 }}>Already have an account? </span>
          <span onClick={onSwitch} style={{ color: v.accent, fontSize: 13, fontWeight: 700, cursor: 'pointer' }}>Sign In</span>
        </div>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// ONBOARDING STEP 1 — Profile & Contacts
// ══════════════════════════════════════════════════════════════
function OnboardStep1({ v, onNext }) {
  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg, display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '36px 24px 20px' }}>
        <StepDots v={v} total={3} current={0} />
        <div style={{ color: v.textMuted, fontSize: 12, fontWeight: 600, letterSpacing: 0.5, marginBottom: 6 }}>STEP 1 OF 3</div>
        <div style={{ color: v.text, fontSize: 24, fontWeight: 800, letterSpacing: -0.5, marginBottom: 6 }}>Tell us about you</div>
        <div style={{ color: v.textSub, fontSize: 14, marginBottom: 28 }}>This helps personalise your experience</div>

        {/* Avatar picker */}
        <div style={{ display: 'flex', justifyContent: 'center', marginBottom: 28 }}>
          <div style={{ position: 'relative' }}>
            <div style={{
              width: 88, height: 88, borderRadius: 26, background: v.accentGrad,
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: 36, boxShadow: `0 8px 28px ${v.accentGlow}`,
              border: `3px solid ${v.cardBorder}`,
            }}>A</div>
            <div style={{
              position: 'absolute', bottom: -4, right: -4,
              width: 28, height: 28, borderRadius: 10, background: v.accentGrad,
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: 14, border: `2px solid ${v.bg}`,
            }}>📷</div>
          </div>
        </div>

        <AuthInput v={v} label="FULL NAME" value="Alex Petrov" />
        <AuthInput v={v} label="USERNAME" placeholder="@username" value="@alex_fit" />
        <AuthInput v={v} label="DATE OF BIRTH" placeholder="DD / MM / YYYY" value="15 / 06 / 1995" />

        {/* Gender */}
        <div style={{ marginBottom: 20 }}>
          <div style={{ color: v.textSub, fontSize: 11, fontWeight: 600, marginBottom: 8, letterSpacing: 0.5 }}>BIOLOGICAL SEX</div>
          <div style={{ display: 'flex', gap: 10 }}>
            {['Male', 'Female', 'Other'].map((g, i) => (
              <div key={g} style={{
                flex: 1, background: i === 0 ? v.accentGrad : v.card,
                border: `1.5px solid ${i === 0 ? 'transparent' : v.cardBorder}`,
                borderRadius: 14, padding: '11px 6px', textAlign: 'center',
                cursor: 'pointer', boxShadow: i === 0 ? `0 4px 14px ${v.accentGlow}` : undefined,
                backdropFilter: v.glass && i !== 0 ? 'blur(20px)' : undefined,
              }}>
                <span style={{ color: i === 0 ? '#fff' : v.textSub, fontSize: 13, fontWeight: 600 }}>{g}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div style={{ padding: '0 24px 32px', marginTop: 'auto' }}>
        <AuthBtn v={v} onClick={onNext}>Continue →</AuthBtn>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// ONBOARDING STEP 2 — Body Stats
// ══════════════════════════════════════════════════════════════
function OnboardStep2({ v, onNext, onBack }) {
  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg, display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '36px 24px 20px' }}>
        <StepDots v={v} total={3} current={1} />
        <div style={{ color: v.textMuted, fontSize: 12, fontWeight: 600, letterSpacing: 0.5, marginBottom: 6 }}>STEP 2 OF 3</div>
        <div style={{ color: v.text, fontSize: 24, fontWeight: 800, letterSpacing: -0.5, marginBottom: 6 }}>Body stats</div>
        <div style={{ color: v.textSub, fontSize: 14, marginBottom: 28 }}>Used to calculate volume and progress</div>

        {/* Weight + Height big pickers */}
        <div style={{ display: 'flex', gap: 12, marginBottom: 20 }}>
          {[['WEIGHT', '82', 'kg'], ['HEIGHT', '183', 'cm']].map(([lbl, val, unit]) => (
            <div key={lbl} style={{
              flex: 1, background: v.card, border: `1.5px solid ${v.cardBorder}`,
              borderRadius: 20, padding: '20px 16px', textAlign: 'center',
              backdropFilter: v.glass ? 'blur(20px)' : undefined,
            }}>
              <div style={{ color: v.textMuted, fontSize: 10, fontWeight: 600, letterSpacing: 0.5, marginBottom: 12 }}>{lbl}</div>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4 }}>
                <div style={{ color: v.textMuted, fontSize: 22, lineHeight: 1 }}>▲</div>
                <div style={{ color: v.text, fontSize: 36, fontWeight: 800, lineHeight: 1 }}>{val}</div>
                <div style={{ color: v.accent, fontSize: 13, fontWeight: 700 }}>{unit}</div>
                <div style={{ color: v.textMuted, fontSize: 22, lineHeight: 1 }}>▼</div>
              </div>
            </div>
          ))}
        </div>

        {/* Unit toggle */}
        <div style={{ background: v.card, border: `1.5px solid ${v.cardBorder}`, borderRadius: 16, padding: '4px', display: 'flex', marginBottom: 20, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
          {['kg / cm', 'lbs / ft'].map((u, i) => (
            <div key={u} style={{
              flex: 1, padding: '10px', borderRadius: 12, textAlign: 'center',
              background: i === 0 ? v.accentGrad : 'transparent',
              boxShadow: i === 0 ? `0 4px 12px ${v.accentGlow}` : undefined,
            }}>
              <span style={{ color: i === 0 ? '#fff' : v.textSub, fontSize: 13, fontWeight: 700 }}>{u}</span>
            </div>
          ))}
        </div>

        <AuthInput v={v} label="CURRENT BODY FAT % (OPTIONAL)" placeholder="e.g. 18%" value="22%" />

        {/* History note */}
        <div style={{
          background: `${v.accent}12`, border: `1px solid ${v.accent}30`,
          borderRadius: 14, padding: '12px 14px', display: 'flex', gap: 10, alignItems: 'flex-start',
        }}>
          <span style={{ fontSize: 16 }}>📈</span>
          <span style={{ color: v.textSub, fontSize: 12, lineHeight: 1.5 }}>
            Your weight history will be tracked over time. You can update it anytime from your Profile.
          </span>
        </div>
      </div>

      <div style={{ padding: '0 24px 32px', marginTop: 'auto', display: 'flex', gap: 10 }}>
        <AuthBtn v={v} secondary onClick={onBack}>← Back</AuthBtn>
        <AuthBtn v={v} onClick={onNext}>Continue →</AuthBtn>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// ONBOARDING STEP 3 — Goals
// ══════════════════════════════════════════════════════════════
function OnboardStep3({ v, onFinish, onBack }) {
  const [selected, setSelected] = React.useState([0]);
  const goals = [
    { icon: '🔥', title: 'Lose Fat', sub: 'Reduce body fat percentage' },
    { icon: '💪', title: 'Build Muscle', sub: 'Increase muscle mass & strength' },
    { icon: '🏃', title: 'Improve Endurance', sub: 'Cardio & stamina training' },
    { icon: '⚡', title: 'Increase Strength', sub: 'Hit new personal records' },
    { icon: '🧘', title: 'Flexibility', sub: 'Yoga, mobility & stretching' },
    { icon: '🌟', title: 'Stay Active', sub: 'General fitness & health' },
  ];
  const toggle = (i) => {
    setSelected(s => s.includes(i) ? s.filter(x => x !== i) : [...s, i]);
  };

  return (
    <div style={{ flex: 1, overflowY: 'auto', background: v.bg, display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '36px 24px 20px' }}>
        <StepDots v={v} total={3} current={2} />
        <div style={{ color: v.textMuted, fontSize: 12, fontWeight: 600, letterSpacing: 0.5, marginBottom: 6 }}>STEP 3 OF 3</div>
        <div style={{ color: v.text, fontSize: 24, fontWeight: 800, letterSpacing: -0.5, marginBottom: 6 }}>What are your goals?</div>
        <div style={{ color: v.textSub, fontSize: 14, marginBottom: 24 }}>Pick one or more — we'll tailor your plan</div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginBottom: 24 }}>
          {goals.map((g, i) => {
            const active = selected.includes(i);
            return (
              <div key={g.title} onClick={() => toggle(i)} style={{
                display: 'flex', alignItems: 'center', gap: 14,
                background: active ? `${v.accent}18` : v.card,
                border: `1.5px solid ${active ? v.accent : v.cardBorder}`,
                borderRadius: 18, padding: '14px 16px', cursor: 'pointer',
                backdropFilter: v.glass ? 'blur(20px)' : undefined,
                transition: 'all 0.15s',
              }}>
                <div style={{
                  width: 44, height: 44, borderRadius: 13, flexShrink: 0,
                  background: active ? v.accentGrad : v.pillBg,
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  fontSize: 20, boxShadow: active ? `0 4px 12px ${v.accentGlow}` : undefined,
                }}>{g.icon}</div>
                <div style={{ flex: 1 }}>
                  <div style={{ color: active ? v.accent : v.text, fontWeight: 700, fontSize: 14 }}>{g.title}</div>
                  <div style={{ color: v.textMuted, fontSize: 12, marginTop: 2 }}>{g.sub}</div>
                </div>
                <div style={{
                  width: 22, height: 22, borderRadius: 8, flexShrink: 0,
                  background: active ? v.accentGrad : 'transparent',
                  border: `2px solid ${active ? 'transparent' : v.cardBorder}`,
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                }}>
                  {active && <span style={{ color: '#fff', fontSize: 12 }}>✓</span>}
                </div>
              </div>
            );
          })}
        </div>

        {/* Weekly frequency */}
        <div style={{ background: v.card, border: `1.5px solid ${v.cardBorder}`, borderRadius: 18, padding: '16px', marginBottom: 20, backdropFilter: v.glass ? 'blur(20px)' : undefined }}>
          <div style={{ color: v.textSub, fontSize: 11, fontWeight: 600, letterSpacing: 0.5, marginBottom: 12 }}>WORKOUTS PER WEEK</div>
          <div style={{ display: 'flex', gap: 8, justifyContent: 'space-between' }}>
            {[2, 3, 4, 5, 6].map((n) => (
              <div key={n} style={{
                flex: 1, padding: '10px 4px', borderRadius: 12, textAlign: 'center',
                background: n === 4 ? v.accentGrad : v.pillBg,
                boxShadow: n === 4 ? `0 4px 12px ${v.accentGlow}` : undefined,
              }}>
                <span style={{ color: n === 4 ? '#fff' : v.textSub, fontSize: 15, fontWeight: 800 }}>{n}×</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div style={{ padding: '0 24px 32px', marginTop: 'auto', display: 'flex', gap: 10 }}>
        <AuthBtn v={v} secondary onClick={onBack}>← Back</AuthBtn>
        <AuthBtn v={v} onClick={onFinish}>Let's Go! 🚀</AuthBtn>
      </div>
    </div>
  );
}

// ══════════════════════════════════════════════════════════════
// AUTH FLOW — full multi-step shell
// ══════════════════════════════════════════════════════════════
function AuthFlow({ variantKey, initialStep = 'login' }) {
  const v = VARIANTS[variantKey];
  const [step, setStep] = React.useState(initialStep);

  return (
    <div style={{
      width: '100%', height: '100%', display: 'flex', flexDirection: 'column',
      background: v.bg, fontFamily: "'Plus Jakarta Sans', sans-serif", overflow: 'hidden',
    }}>
      {step === 'login'    && <LoginScreen    v={v} onSwitch={() => setStep('register')} />}
      {step === 'register' && <RegisterScreen v={v} onSwitch={() => setStep('login')} onNext={() => setStep('step1')} />}
      {step === 'step1'    && <OnboardStep1   v={v} onNext={() => setStep('step2')} />}
      {step === 'step2'    && <OnboardStep2   v={v} onNext={() => setStep('step3')} onBack={() => setStep('step1')} />}
      {step === 'step3'    && <OnboardStep3   v={v} onFinish={() => setStep('login')} onBack={() => setStep('step2')} />}
    </div>
  );
}

Object.assign(window, { AuthFlow, LoginScreen, RegisterScreen, OnboardStep1, OnboardStep2, OnboardStep3 });
