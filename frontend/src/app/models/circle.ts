export interface Circle {
  id: string;
  name: string;
  contributionAmount: number;
  maxMembers: number;
  contributionIntervalDays: number;
  status: 'Forming' | 'Active' | 'Completed';
  organizerId: string;
  startedAt: string | null;
  createdAt: string;
  completedAt: string | null;
}
export interface CreateCircleRequest {
  name: string;
  contributionAmount: number;
  maxMembers: number;
  contributionIntervalDays: number;
}