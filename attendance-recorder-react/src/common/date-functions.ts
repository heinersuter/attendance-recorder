export function getWeekdayName(date: Date): string {
  return new Intl.DateTimeFormat('en-US', { weekday: 'long' }).format(date);
}

export function getInvariantDateString(date: Date): string {
  return date.toISOString().slice(0, 10);
}
