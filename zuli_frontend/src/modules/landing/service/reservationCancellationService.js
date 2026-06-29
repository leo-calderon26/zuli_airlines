import axios from "axios";

const API = "/api/ReservationCancellation";

export const requestCancellation = async (reservationCode) =>
  (await axios.post(`${API}/request`, { ReservationCode: reservationCode })).data;

export const confirmCancellation = async (token) =>
  (await axios.post(`${API}/confirm`, { Token: token })).data;
