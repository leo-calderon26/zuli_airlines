<script setup>
import { onMounted, ref, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useUser } from '../composable/useUser';
import { useUserStore } from '../store/userStore';
import AppTable from '../../../shared/AppTable.vue';

const router = useRouter();
const userStore = useUserStore();
const { fetchUsersPaginated } = useUser();
const canCreateUsers = sessionStorage.getItem('userRole') === 'Administrator';
const canEditUsers = Boolean(sessionStorage.getItem('userRole'));

const search = ref('');
const searchType = ref('name');
let searchTimeoutId = null;

onMounted(async () => {
    await fetchUsersPaginated(1, userStore.pageSize);
});

onUnmounted(() => {
    if (searchTimeoutId) clearTimeout(searchTimeoutId);
});

function handleSearchInput() {
    if (searchTimeoutId) clearTimeout(searchTimeoutId);
    searchTimeoutId = setTimeout(() => {
        searchUsers();
    }, 300);
}

async function searchUsers() {
    await fetchUsersPaginated(1, userStore.pageSize, search.value, searchType.value);
}

async function handlePageChange(newPage) {
    await fetchUsersPaginated(newPage, userStore.pageSize, search.value, searchType.value);
}

function goToCreateUser() {
    router.push({ name: 'userCreate' });
}

function cacheUserForEdit(user) {
    sessionStorage.setItem('userEditData', JSON.stringify(user));
}

function goToUserEdit(user) {
    cacheUserForEdit(user);
    router.push({
        name: 'userEdit',
        params: { userId: user.userId },
        state: { user }
    });
}

function formatRole(userRole) {
    if (userRole === 'Administrator') return 'Administrador';
    if (userRole === 'Operator') return 'Operador';
    return userRole;
}
</script>

<template>
<AppTable
    :total-records="userStore.totalRecords"
    :page-number="userStore.pageNumber"
    :total-pages="userStore.totalPages"
    :page-size="userStore.pageSize"
    :empty="userStore.users.length === 0"
    empty-text="No se encontraron usuarios."
    @change-page="handlePageChange"
>
    <template #header>
        <div class="flex items-center gap-2">
            <label for="search-users" class="sr-only">Buscar</label>
            <div class="relative">
                <div class="absolute inset-y-0 inset-s-0 flex items-center ps-3 pointer-events-none">
                    <svg class="w-4 h-4 text-gray-500" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
                </div>
                <input
                    v-model="search"
                    type="text"
                    id="search-users"
                    class="block w-full max-w-72 ps-9 pe-3 py-2 border border-gray-300 bg-white text-sm text-gray-900 rounded-lg shadow-sm placeholder:text-gray-500 focus:border-gold focus:ring-1 focus:ring-gold"
                    placeholder="Buscar usuario"
                    @input="handleSearchInput"
                />
            </div>
            <select
                v-model="searchType"
                class="h-9 w-auto rounded-lg border border-gray-300 bg-white px-3 text-sm text-gray-700 outline-none hover:cursor-pointer focus:border-gold focus:ring-1 focus:ring-gold"
                @change="searchUsers"
            >
                <option value="name">Nombre</option>
                <option value="email">Correo</option>
                <option value="nationalId">Cédula</option>
            </select>
        </div>
        <button
            v-if="canCreateUsers"
            type="button"
            class="bg-primary hover:bg-select text-white font-semibold hover:text-white py-2 px-4 border hover:border-transparent rounded hover:cursor-pointer text-center"
            @click="goToCreateUser"
        >
            Agregar
        </button>
    </template>

    <template #thead>
        <th scope="col" class="px-8 py-4 font-medium">Nombre</th>
        <th scope="col" class="px-8 py-4 font-medium">Apellidos</th>
        <th scope="col" class="px-8 py-4 font-medium">Correo</th>
        <th scope="col" class="px-8 py-4 font-medium">Rol</th>
        <th v-if="canEditUsers" scope="col" class="px-8 py-4 font-medium">Editar</th>
    </template>

    <tr v-for="user in userStore.users" :key="user.userId" class="border-b border-gray-200 bg-white hover:bg-gray-50">
        <th scope="row" class="whitespace-nowrap px-8 py-5 font-medium text-gray-900">
            <p>{{ user.firstName }}</p>
        </th>
        <td class="px-8 py-5">
            <p>{{ user.firstLastName }} {{ user.secondLastName }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ user.businessEmail }}</p>
        </td>
        <td class="px-8 py-5">
            <p>{{ formatRole(user.userRole) }}</p>
        </td>
        <td v-if="canEditUsers" class="px-8 py-5">
            <button
                type="button"
                class="font-medium text-gold hover:underline"
                @click="goToUserEdit(user)"
            >
                Edit
            </button>
        </td>
    </tr>
</AppTable>
</template>
