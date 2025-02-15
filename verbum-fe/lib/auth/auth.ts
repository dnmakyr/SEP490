/* eslint-disable @typescript-eslint/no-explicit-any */
import jwt from 'jsonwebtoken'
import type { User } from '~/types/user'

const isUser = (decoded: any): decoded is User => {
  return decoded && typeof decoded.email === 'string'
}

export const decodeToken = (token: string | undefined | null): User | null => {
  if (!token) {
    return null
  }
  const decoded = jwt.decode(token) as any
  const { exp, iss, aud, ...user } = decoded
  if (isUser(user)) {
    return user
  }
  return null
}

export const isTokenExpired = (token: string | undefined | null): boolean => {
  try {
    if (!token) {
      throw new Error('Token is undefined or null');
    }
    const decoded = jwt.decode(token); // Decode token and extract expiration
    const { exp } = decoded as any;
    const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
    return exp < currentTime; // Token is expired if `exp` is earlier than now
  } catch (error) {
    console.error('Invalid token:', error);
    return true; // Treat invalid tokens as expired
  }
}