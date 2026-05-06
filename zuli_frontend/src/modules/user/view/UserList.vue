<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <PublicNavBar />

        <main class="flex flex-1 flex-col bg-[var(--color-secondary)] px-8 pb-32 pt-14">
            <section class="mx-auto flex w-full max-w-[1280px] flex-col gap-10">
                <div class="flex flex-col gap-5 lg:flex-row lg:items-center lg:justify-between">
                    <div class="flex h-12 w-full overflow-hidden rounded-full border border-gray-300 bg-white lg:max-w-[990px]">
                        <input
                            v-model="search"
                            class="h-full flex-1 bg-white px-7 text-[16px] text-[var(--color-content)] outline-none placeholder:text-gray-400"
                            placeholder="Buscar usuario"
                            type="text"
                            @input="handleSearchInput"
                        />

                        <select
                            v-model="searchType"
                            class="h-full w-[150px] border-l border-gray-200 bg-white px-3 text-sm font-semibold text-[var(--color-content)] outline-none hover:cursor-pointer"
                            @change="searchUsers"
                        >
                            <option value="name">Nombre</option>
                            <option value="email">Correo</option>
                            <option value="nationalId">Cédula</option>
                        </select>

                        <button
                            class="flex h-full w-14 items-center justify-center text-[var(--color-content)] transition hover:cursor-pointer hover:brightness-75"
                            type="button"
                            @click="searchUsers"
                        >
                            <svg
                                class="h-7 w-7"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                stroke-width="2"
                            >
                                <circle cx="11" cy="11" r="7"></circle>
                                <path d="M20 20L16.5 16.5"></path>
                            </svg>
                        </button>
                    </div>

                    <button
                        class="h-12 rounded-lg bg-[var(--color-primary)] px-8 text-[18px] font-bold text-white shadow-md transition hover:cursor-pointer hover:brightness-75"
                        type="button"
                        @click="goToCreateUser"
                    >
                        Agregar Usuario
                    </button>
                </div>

                <section class="min-h-[520px] rounded-lg border border-gray-300 bg-white px-8 py-4">
                    <div class="overflow-x-auto">
                        <table class="w-full min-w-[900px] border-collapse text-center">
                            <thead>
                                <tr class="border-b-4 border-[var(--color-purple)] text-[20px] font-bold text-black">
                                    <th class="px-4 py-3">Nombre</th>
                                    <th class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        Apellidos
                                    </th>
                                    <th class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        Cédula
                                    </th>
                                    <th class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        Rol
                                    </th>
                                    <th class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        Detalles
                                    </th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr
                                    v-for="user in users"
                                    :key="user.userId"
                                    class="border-b-4 border-[var(--color-purple)] text-[16px] font-bold text-black"
                                >
                                    <td class="px-4 py-3">
                                        {{ user.firstName }}
                                    </td>

                                    <td class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        {{ user.firstLastName }} {{ user.secondLastName }}
                                    </td>

                                    <td class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        {{ user.nationalId }}
                                    </td>

                                    <td class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        {{ formatRole(user.userRole) }}
                                    </td>

                                    <td class="border-l-4 border-[var(--color-purple)] px-4 py-3">
                                        <button
                                            class="inline-flex items-center gap-2 font-bold text-[var(--color-content)] transition hover:cursor-pointer hover:brightness-75"
                                            type="button"
                                            @click="goToUserDetails(user.userId)"
                                        >
                                            Detalles
                                            <span class="flex h-8 w-8 items-center justify-center border border-gray-400 text-2xl leading-none">
                                                →
                                            </span>
                                        </button>
                                    </td>
                                </tr>

                                <tr v-if="!isLoading && users.length === 0">
                                    <td
                                        class="px-4 py-10 text-center text-[17px] font-semibold text-gray-500"
                                        colspan="5"
                                    >
                                        No se encontraron usuarios.
                                    </td>
                                </tr>

                                <tr v-if="isLoading">
                                    <td
                                        class="px-4 py-10 text-center text-[17px] font-semibold text-gray-500"
                                        colspan="5"
                                    >
                                        Cargando usuarios...
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <p
                        v-if="errorMessage"
                        class="mt-5 rounded-md border border-red-300 bg-red-50 px-4 py-3 text-center text-sm font-bold text-red-700"
                    >
                        {{ errorMessage }}
                    </p>

                    <div class="mt-6 flex flex-col items-center justify-between gap-4 border-t border-gray-200 pt-5 md:flex-row">
                        <p class="text-sm font-semibold text-[var(--color-content)]">
                            Página {{ page }} de {{ totalPages }} —
                            {{ totalItems }} usuario(s)
                        </p>

                        <div class="flex items-center gap-3">
                            <button
                                class="h-10 rounded-md border border-[var(--color-primary)] px-4 font-semibold text-[var(--color-primary)] transition hover:cursor-pointer hover:brightness-75 disabled:cursor-not-allowed disabled:opacity-50"
                                :disabled="page <= 1 || isLoading"
                                type="button"
                                @click="previousPage"
                            >
                                Anterior
                            </button>

                            <button
                                class="h-10 rounded-md border border-[var(--color-primary)] px-4 font-semibold text-[var(--color-primary)] transition hover:cursor-pointer hover:brightness-75 disabled:cursor-not-allowed disabled:opacity-50"
                                :disabled="page >= totalPages || isLoading"
                                type="button"
                                @click="nextPage"
                            >
                                Siguiente
                            </button>
                        </div>
                    </div>
                </section>
            </section>
        </main>

        <PublicBottomBar />
    </div>
</template>

<script>
import PublicNavBar from "../../../shared/PublicNavBar.vue";
import PublicBottomBar from "../../../shared/PublicBottomBar.vue";
import userService from "../services/userService";

export default {
    name: "UserList",

    components: {
        PublicNavBar,
        PublicBottomBar
    },

    data() {
        return {
            users: [],
            search: "",
            searchType: "name",
            page: 1,
            pageSize: 10,
            totalItems: 0,
            totalPages: 1,
            isLoading: false,
            errorMessage: "",
            searchTimeoutId: null
        };
    },

    mounted() {
        this.loadUsers();
    },

    beforeUnmount() {
        if (this.searchTimeoutId) {
            clearTimeout(this.searchTimeoutId);
        }
    },

    methods: {
        async loadUsers() {
            this.isLoading = true;
            this.errorMessage = "";

            try {
                const result = await userService.getUsers({
                    searchType: this.searchType,
                    search: this.search,
                    page: this.page,
                    pageSize: this.pageSize
                });

                this.users = result.users || [];
                this.totalItems = result.totalItems || 0;
                this.totalPages = result.totalPages || 1;
                this.page = result.page || 1;
            } catch (error) {
                this.errorMessage = error.message || "No se pudieron cargar los usuarios.";
            } finally {
                this.isLoading = false;
            }
        },

        handleSearchInput() {
            if (this.searchTimeoutId) {
                clearTimeout(this.searchTimeoutId);
            }

            this.searchTimeoutId = setTimeout(() => {
                this.searchUsers();
            }, 300);
        },

        searchUsers() {
            this.page = 1;
            this.loadUsers();
        },

        previousPage() {
            if (this.page <= 1) {
                return;
            }

            this.page -= 1;
            this.loadUsers();
        },

        nextPage() {
            if (this.page >= this.totalPages) {
                return;
            }

            this.page += 1;
            this.loadUsers();
        },

        goToCreateUser() {
            this.$router.push({ name: "userCreate" });
        },

        goToUserDetails(userId) {
            console.log("User details pending:", userId);
        },

        formatRole(userRole) {
            if (userRole === "Administrator") {
                return "Administrador";
            }

            if (userRole === "Operator") {
                return "Operador";
            }

            return userRole;
        }
    }
};
</script>