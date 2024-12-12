export interface Place {
  id: number;
  roomsCount: number;
  size: number;
  address: string;
  description?: string;
  price: number;
  averageRating?: number;
  userId: string;
}

export interface Reservation {
  id: number;
  placeId: number;
  createdAt: Date;
  startDate: Date;
  endDate: Date;
  status: string;
  price: number;
  place: Place;
  user: User;
  review?: Review | null;
}

export interface Review {
  id: number;
  comment: string;
  rating: number;
  reservationId: number;
  user: User | undefined;
}

export interface User {
  id: string;
  userName: string;
  email: string;
}
