export interface Circle {
  id: string | number;
  name: string;
  contributionAmount: number;
  maxMembers: number;
  contributionIntervalDays: number;
  status: 'NotStarted' | 'Forming' | 'Active' | 'Completed';
  organizerId: number | string;
  startedAt?: string | null;
  createdAt?: string;
  completedAt?: string | null;
}

export interface CreateCircleRequest {
  name: string;
  contributionAmount: number;
  maxMembers: number;
  contributionIntervalDays: number;
  organizerId: number;
}