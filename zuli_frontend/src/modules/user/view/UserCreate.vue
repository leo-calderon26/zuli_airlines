<template>
    <div class="flex min-h-svh flex-col bg-[var(--color-secondary)]">
        <main class="flex-1 bg-[var(--color-secondary)] px-6 pb-32 pt-8">
            <section class="relative mx-auto flex min-h-[690px] max-w-6xl justify-center pt-8">
                <div class="absolute left-[7%] right-[5%] top-[175px] h-px bg-[var(--color-purple)]"></div>

                <div class="relative z-10 w-full max-w-180">
                    <h2 class="mb-10 text-[15px] font-medium text-content">
                        Crear Nuevo Usuario
                    </h2>

                    <UserForm />
                </div>
            </section>
        </main>
    </div>
</template>

<script setup>
import { reactive } from "vue";
import { useRouter } from "vue-router";
import ErrorModal from "../../../shared/ErrorModal.vue";
import SuccessModal from "../../../shared/SuccessModal.vue";
import AppButton from "../../../shared/AppButton.vue";
import AppInput from "../../../shared/AppInput.vue";
import { useForm } from "../../../shared/useForm.js";
import userService from "../services/userService";
import UserForm from "../components/UserForm.vue";
</script>
