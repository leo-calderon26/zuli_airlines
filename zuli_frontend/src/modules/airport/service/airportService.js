import axios from "axios"

const API_URL = "/api/Airport"

export const getAirports = async () => {
    try {
        const response = await axios.get(`${API_URL}/GetAll`)
        return response.data
    } catch (error) {
        console.error("Error al obtener aeropuertos:", error)
        throw error
    }
}